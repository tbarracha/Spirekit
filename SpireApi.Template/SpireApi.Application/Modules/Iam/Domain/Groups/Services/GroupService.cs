using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireCore.API.EntityFramework.Entities.Memberships;
using SpireCore.Constants;
using SpireCore.Services;
using System.Linq;
using System.Linq.Expressions;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Services;

/// <summary>
/// Domain-level orchestration around <see cref="Group"/>, <see cref="GroupMember"/>,
/// <see cref="GroupType"/>, … Delegates persistence to the repositories in
/// <see cref="GroupRepositoryContext"/>.
/// </summary>
public sealed class GroupService
    : BaseGroupMembershipService<Guid, Guid, Group, GroupType,
                                 GroupMember, GroupMembershipState, GroupMemberAudit>,
      ITransientService
{
    private readonly GroupRepositoryContext _ctx;

    public GroupService(GroupRepositoryContext groupRepositoryContext)
        => _ctx = groupRepositoryContext;

    #region ── Group CRUD ──────────────────────────────────────────────────────────

    public override async Task<Group> CreateGroupAsync(
        Guid ownerUserId,
        string name,
        Guid groupTypeId,
        string? description = null,
        Guid parentGroupId = default)
    {
        var type = await GetGroupTypeByIdAsync(groupTypeId)
                   ?? throw new KeyNotFoundException($"GroupType {groupTypeId} not found.");

        // Enforce unique name within the same parent (example rule – tweak to your needs)
        if (await _ctx.GroupRepository.ExistsAsync(g =>
                g.ParentGroupId == parentGroupId && g.Name == name && g.StateFlag == StateFlags.ACTIVE))
            throw new InvalidOperationException($"Group name “{name}” already exists in that scope.");

        var now = DateTime.UtcNow;
        var group = new Group
        {
            Id = Guid.NewGuid(),
            OwnerUserId = ownerUserId,
            Name = name,
            GroupTypeId = groupTypeId,
            ParentGroupId = parentGroupId == Guid.Empty ? Guid.Empty : parentGroupId,
            Description = description,
            CreatedAt = now,
            UpdatedAt = now,
            StateFlag = StateFlags.ACTIVE
        };

        await _ctx.GroupRepository.AddAsync(group);
        return group;
    }

    public override Task<Group?> GetGroupByIdAsync(Guid groupId)
        => _ctx.GroupRepository.GetByIdAsync(groupId);

    public override Task<IReadOnlyList<Group>> GetChildGroupsAsync(Guid parentGroupId)
        => _ctx.GroupRepository.ListFilteredAsync(g =>
               g.ParentGroupId == parentGroupId && g.StateFlag == StateFlags.ACTIVE);

    public override async Task<Group?> GetParentGroupAsync(Guid groupId)
    {
        var grp = await GetGroupByIdAsync(groupId);
        return grp?.ParentGroupId is { } pid
            ? await GetGroupByIdAsync(pid)
            : null;
    }

    public override Task<bool> GroupExistsAsync(Guid groupId)
        => _ctx.GroupRepository.ExistsAsync(g => g.Id == groupId && g.StateFlag == StateFlags.ACTIVE);

    public override Task<IReadOnlyList<Group>> ListGroupsByOwnerAsync(Guid ownerUserId)
        => _ctx.GroupRepository.ListFilteredAsync(g =>
               g.OwnerUserId == ownerUserId && g.StateFlag == StateFlags.ACTIVE);

    public override async Task<Group?> GetOwnedGroupAsync(Guid ownerUserId)
        => (await ListGroupsByOwnerAsync(ownerUserId)).FirstOrDefault();

    #endregion

    #region ── Group-Type queries ─────────────────────────────────────────────────

    public override Task<GroupType?> GetGroupTypeByIdAsync(Guid typeId)
        => _ctx.GroupTypeRepository.GetByIdAsync(typeId);

    public override Task<IReadOnlyList<GroupType>> ListGroupTypesAsync()
        => _ctx.GroupTypeRepository.ListFilteredAsync(gt => gt.StateFlag == StateFlags.ACTIVE);

    #endregion

    #region ── Member management ──────────────────────────────────────────────────
    
    public override async Task<IReadOnlyList<Group>> ListGroupsForUserAsync(Guid userId)
    {
        // 1. All active memberships for this user
        var memberships = await _ctx.GroupMemberRepository.ListFilteredAsync(m =>
            m.UserId == userId && m.StateFlag == StateFlags.ACTIVE);

        if (memberships.Count == 0)
            return Array.Empty<Group>();

        // 2. Distinct group ids the user belongs to
        var groupIds = memberships.Select(m => m.GroupId).Distinct().ToList();

        // 3. Fetch the corresponding active groups
        return await _ctx.GroupRepository.ListFilteredAsync(g =>
            groupIds.Contains(g.Id) && g.StateFlag == StateFlags.ACTIVE);
    }

    public override async Task<GroupMember> AddMemberAsync(
        Guid groupId,
        Guid userId,
        Guid moderatorUserId = default,
        string? reason = null)
    {
        if (!await GroupExistsAsync(groupId))
            throw new KeyNotFoundException($"Group {groupId} not found.");

        if (await UserIsGroupMemberAsync(groupId, userId))
            throw new InvalidOperationException("User is already a member of that group.");

        var state = await _ctx.GroupMembershipStateRepository
                      .GetFilteredAsync(s => s.IsDefault && s.StateFlag == StateFlags.ACTIVE)
                   ?? throw new InvalidOperationException("No default membership state configured.");

        var now = DateTime.UtcNow;
        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            UserId = userId,
            StateId = state.Id,
            CreatedAt = now,
            UpdatedAt = now,
            StateFlag = StateFlags.ACTIVE
        };

        await _ctx.GroupMemberRepository.AddAsync(member);

        await _ctx.GroupMemberAuditRepository.AddAsync(new GroupMemberAudit
        {
            Id = Guid.NewGuid(),
            GroupMemberUserId = member.Id,
            GroupId = groupId,
            PerformedByUserId = moderatorUserId == Guid.Empty ? userId : moderatorUserId,
            Action = GroupMemberAuditType.Join,
            Reason = reason,
            CreatedAt = now,
            StateFlag = StateFlags.ACTIVE
        });

        return member;
    }

    public override async Task<IReadOnlyList<GroupMember>> AddMembersBulkAsync(
        Guid groupId,
        IEnumerable<Guid> userIds,
        Guid moderatorUserId = default,
        string? reason = null)
    {
        var added = new List<GroupMember>();
        foreach (var uid in userIds.Distinct())
            added.Add(await AddMemberAsync(groupId, uid, moderatorUserId, reason));

        return added;
    }

    public override async Task RemoveMemberAsync(
        Guid groupMemberId,
        Guid moderatorUserId = default,
        string? reason = null)
    {
        var member = await GetGroupMemberByIdAsync(groupMemberId)
                     ?? throw new KeyNotFoundException("Group member not found.");

        member.StateFlag = StateFlags.DELETED;
        member.UpdatedAt = DateTime.UtcNow;
        await _ctx.GroupMemberRepository.UpdateAsync(member);

        await _ctx.GroupMemberAuditRepository.AddAsync(new GroupMemberAudit
        {
            Id = Guid.NewGuid(),
            GroupMemberUserId = member.Id,
            GroupId = member.GroupId,
            PerformedByUserId = moderatorUserId == Guid.Empty ? member.UserId : moderatorUserId,
            Action = GroupMemberAuditType.Leave,
            Reason = reason,
            CreatedAt = DateTime.UtcNow,
            StateFlag = StateFlags.ACTIVE
        });
    }

    public override async Task RemoveMembersBulkAsync(
        IEnumerable<Guid> groupMemberIds,
        Guid moderatorUserId = default,
        string? reason = null)
    {
        foreach (var id in groupMemberIds)
            await RemoveMemberAsync(id, moderatorUserId, reason);
    }

    public override Task<GroupMember?> GetGroupMemberByIdAsync(Guid groupMemberId)
        => _ctx.GroupMemberRepository.GetByIdAsync(groupMemberId);

    public override Task<GroupMember?> GetMemberByUserIdAsync(Guid groupId, Guid userId)
        => _ctx.GroupMemberRepository.FirstOrDefaultAsync(m =>
               m.GroupId == groupId && m.UserId == userId && m.StateFlag == StateFlags.ACTIVE);

    public override async Task<DateTime?> GetMemberJoinDateAsync(Guid groupId, Guid userId)
        => (await GetMemberByUserIdAsync(groupId, userId))?.CreatedAt;

    public override Task<IReadOnlyList<GroupMember>> ListGroupMembersAsync(Guid groupId)
        => _ctx.GroupMemberRepository.ListFilteredAsync(m =>
               m.GroupId == groupId && m.StateFlag == StateFlags.ACTIVE);

    public override Task<IReadOnlyList<GroupMember>> ListGroupModeratorsAsync(Guid groupId)
        => _ctx.GroupMemberRepository.ListFilteredAsync(m =>
               m.GroupId == groupId && m.RoleId == Guid.Empty && m.StateFlag == StateFlags.ACTIVE);

    public override Task<IReadOnlyList<GroupMember>> ListMembersByStateAsync(
        Guid groupId, MembershipState participationState)
        => _ctx.GroupMemberRepository.ListFilteredAsync(m =>
               m.GroupId == groupId && m.CurrentState == participationState);

    public override async Task<IReadOnlyList<GroupMember>> ListBannedMembersAsync(Guid groupId)
        => await ListMembersByStateAsync(groupId, MembershipState.Banned);

    public override Task<int> CountGroupMembersAsync(
        Guid groupId,
        MembershipState? participationState = null)
    {
        Expression<Func<GroupMember, bool>> predicate = participationState is null
            ? m => m.GroupId == groupId && m.StateFlag == StateFlags.ACTIVE
            : m => m.GroupId == groupId && m.CurrentState == participationState;

        return _ctx.GroupMemberRepository.CountAsync(predicate);
    }

    public override async Task<IReadOnlyList<GroupMember>> ListUserGroupsAsync(Guid userId)
        => await _ctx.GroupMemberRepository.ListFilteredAsync(m =>
               m.UserId == userId && m.StateFlag == StateFlags.ACTIVE);

    public override Task<int> CountGroupsForUserAsync(Guid userId)
        => _ctx.GroupMemberRepository.CountAsync(m =>
               m.UserId == userId && m.StateFlag == StateFlags.ACTIVE);

    public override Task<bool> UserIsGroupMemberAsync(Guid groupId, Guid userId)
        => _ctx.GroupMemberRepository.ExistsAsync(m =>
               m.GroupId == groupId && m.UserId == userId && m.StateFlag == StateFlags.ACTIVE);

    #endregion

    #region ── Roles ──────────────────────────────────────────────────────────────

    public override async Task AssignRoleAsync(
        Guid groupMemberId,
        Guid roleId,
        Guid moderatorUserId = default,
        string? reason = null)
    {
        var member = await GetGroupMemberByIdAsync(groupMemberId)
                     ?? throw new KeyNotFoundException("Member not found.");

        member.RoleId = roleId;
        member.UpdatedAt = DateTime.UtcNow;
        await _ctx.GroupMemberRepository.UpdateAsync(member);

        await _ctx.GroupMemberAuditRepository.AddAsync(new GroupMemberAudit
        {
            Id = Guid.NewGuid(),
            GroupId = member.GroupId,
            GroupMemberUserId = member.UserId,
            PerformedByUserId = moderatorUserId == Guid.Empty ? member.UserId : moderatorUserId,
            Action = GroupMemberAuditType.RoleChanged,
            Reason = reason,
            CreatedAt = DateTime.UtcNow,
            StateFlag = StateFlags.ACTIVE
        });
    }

    public override async Task AssignRoleBulkAsync(
        IEnumerable<Guid> groupMemberIds,
        Guid roleId,
        Guid moderatorUserId = default,
        string? reason = null)
    {
        foreach (var id in groupMemberIds)
            await AssignRoleAsync(id, roleId, moderatorUserId, reason);
    }

    public override async Task RemoveRoleAsync(
        Guid groupMemberId,
        Guid moderatorUserId = default,
        string? reason = null)
        => await AssignRoleAsync(groupMemberId, Guid.Empty, moderatorUserId, reason);

    public override async Task<bool> UserHasRoleAsync(
        Guid groupId,
        Guid userId,
        Guid roleId)
        => await _ctx.GroupMemberRepository.ExistsAsync(m =>
               m.GroupId == groupId
               && m.UserId == userId
               && m.RoleId == roleId
               && m.StateFlag == StateFlags.ACTIVE);

    #endregion

    #region ── Moderation / audits ────────────────────────────────────────────────

    public override async Task ModerateMemberAsync(
        Guid groupMemberId,
        MembershipState newState,
        Guid moderatorUserId = default,
        string? reason = null)
    {
        var member = await GetGroupMemberByIdAsync(groupMemberId)
                     ?? throw new KeyNotFoundException();

        member.CurrentState = newState;
        member.UpdatedAt = DateTime.UtcNow;
        await _ctx.GroupMemberRepository.UpdateAsync(member);

        await _ctx.GroupMemberAuditRepository.AddAsync(new GroupMemberAudit
        {
            Id = Guid.NewGuid(),
            GroupId = member.GroupId,
            GroupMemberUserId = member.UserId,
            NewState = newState,
            PerformedByUserId = moderatorUserId == Guid.Empty ? member.UserId : moderatorUserId,
            Action = GroupMemberAuditType.StateChanged,
            Reason = reason,
            CreatedAt = DateTime.UtcNow,
            StateFlag = StateFlags.ACTIVE
        });
    }

    public override Task<GroupMemberAudit?> GetLatestMemberAuditAsync(Guid groupMemberId)
        => _ctx.GroupMemberAuditRepository.FirstOrDefaultAsync(a =>
               a.GroupMemberUserId == groupMemberId && a.StateFlag == StateFlags.ACTIVE);

    public override Task<IReadOnlyList<GroupMemberAudit>> ListMemberAuditsAsync(Guid groupMemberId)
        => _ctx.GroupMemberAuditRepository.ListFilteredAsync(a =>
               a.GroupMemberUserId == groupMemberId && a.StateFlag == StateFlags.ACTIVE);

    public override Task<IReadOnlyList<GroupMemberAudit>> ListGroupAuditsAsync(Guid groupId)
        => _ctx.GroupMemberAuditRepository.ListFilteredAsync(a =>
               a.GroupId == groupId && a.StateFlag == StateFlags.ACTIVE);

    public override Task<IReadOnlyList<GroupMembershipState>> ListMembershipStatesForMemberAsync(
        Guid groupMemberId)
        => _ctx.GroupMembershipStateRepository.ListFilteredAsync(s =>
               s.GroupMemberId == groupMemberId);

    public override Task<GroupMembershipState?> GetGroupMembershipStateAsync(Guid stateId)
        => _ctx.GroupMembershipStateRepository.GetByIdAsync(stateId);

    #endregion

    #region ── Ownership transfer ────────────────────────────────────────────────

    public override async Task TransferGroupOwnershipAsync(
        Guid groupId,
        Guid newOwnerUserId,
        Guid performedByUserId = default,
        string? reason = null)
    {
        var group = await GetGroupByIdAsync(groupId)
                    ?? throw new KeyNotFoundException("Group not found.");

        group.OwnerUserId = newOwnerUserId;
        group.UpdatedAt = DateTime.UtcNow;
        await _ctx.GroupRepository.UpdateAsync(group);

        // Optional: audit ownership change
        await _ctx.GroupMemberAuditRepository.AddAsync(new GroupMemberAudit
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            GroupMemberUserId = newOwnerUserId,
            PerformedByUserId = performedByUserId == Guid.Empty ? newOwnerUserId : performedByUserId,
            Action = GroupMemberAuditType.OwnershipTransferred,
            Reason = reason,
            CreatedAt = DateTime.UtcNow,
            StateFlag = StateFlags.ACTIVE
        });
    }

    public override Task<bool> UserIsGroupOwnerAsync(Guid groupId, Guid userId)
        => _ctx.GroupRepository.ExistsAsync(g =>
               g.Id == groupId && g.OwnerUserId == userId && g.StateFlag == StateFlags.ACTIVE);

    #endregion
}

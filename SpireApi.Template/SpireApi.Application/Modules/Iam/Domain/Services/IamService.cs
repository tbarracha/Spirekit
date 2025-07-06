using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Users;
using SpireApi.Application.Modules.Iam.Domain.Models.Users.Repositories;
using SpireApi.Application.Modules.Iam.Repositories;
using SpireApi.Shared.EntityFramework.Entities.Memberships;
using SpireCore.Constants;
using SpireCore.Services;

namespace SpireApi.Application.Modules.Iam.Domain.Services;

/// <summary>
/// Central IAM domain service for managing groups, members, roles, permissions, and audits, using the current domain models.
/// </summary>
public class IamService : ITransientService
{
    private readonly IamUserRepository _iamUserRepository;
    private readonly GroupRepository _groupRepository;
    private readonly GroupTypeRepository _groupTypeRepository;
    private readonly GroupMemberRepository _groupMemberRepository;
    private readonly GroupMembershipStateRepository _groupMembershipStateRepository;
    private readonly GroupMemberAuditRepository _moderationAuditRepository;
    private readonly RoleRepository _roleRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RolePermissionRepository _rolePermissionRepository;
    private readonly PermissionRepository _permissionRepository;
    private readonly PermissionScopeRepository _permissionScopeRepository;

    public IamService(
        IamUserRepository iamUserRepository,
        GroupRepository groupRepository,
        GroupTypeRepository groupTypeRepository,
        GroupMemberRepository groupMemberRepository,
        GroupMemberAuditRepository moderationAuditRepository,
        RoleRepository roleRepository,
        UserRoleRepository userRoleRepository,
        RolePermissionRepository rolePermissionRepository,
        PermissionRepository permissionRepository,
        PermissionScopeRepository permissionScopeRepository)
    {
        _iamUserRepository = iamUserRepository;
        _groupRepository = groupRepository;
        _groupTypeRepository = groupTypeRepository;
        _groupMemberRepository = groupMemberRepository;
        _moderationAuditRepository = moderationAuditRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _permissionScopeRepository = permissionScopeRepository;
    }

    public const string DEFAULT_REASON_MEMBER_ADDED = "Member added";
    public const string DEFAULT_REASON_MEMBER_REMOVED = "Member removed";

    /// <summary>
    /// Creates a new private team group for a user and enrolls them as owner/member.
    /// </summary>
    public async Task<(IamUser user, Group group)> NewUserSetup(Guid userId)
    {
        var user = await _iamUserRepository.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        var teamGroupType = await _groupTypeRepository.GetFilteredAsync(
            gt => gt.Name.ToLower() == "team",
            state: StateFlags.ACTIVE
        ) ?? throw new Exception("GroupType 'TEAM' not found");

        var group = new Group
        {
            Id = Guid.NewGuid(),
            OwnerId = userId,
            GroupTypeId = teamGroupType.Id,
            Name = $"{user.DisplayName}'s Team",
            Description = $"Private team for {user.DisplayName}",
            StateFlag = StateFlags.ACTIVE
        };

        await _groupRepository.AddAsync(group);

        var membershipState = new GroupMembershipState
        {
            Id = Guid.NewGuid(),
            State = MembershipState.Active
        };
        await _groupMembershipStateRepository.AddAsync(membershipState);

        var groupMember = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = user.Id,
            MembershipState = membershipState,
            StateFlag = StateFlags.ACTIVE
        };

        await _groupMemberRepository.AddAsync(groupMember);

        return (user, group);
    }

    #region Groups

    /// <summary>
    /// Creates a group for a user with specified name and description.
    /// </summary>
    public async Task<Group> CreateGroupForUserAsync(Guid userId, string groupName, string? description = null)
    {
        var user = await _iamUserRepository.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        var teamGroupType = await _groupTypeRepository.GetFilteredAsync(
            gt => gt.Name.ToUpperInvariant() == "TEAM",
            state: StateFlags.ACTIVE
        ) ?? throw new Exception("GroupType 'TEAM' not found");

        var group = new Group
        {
            Id = Guid.NewGuid(),
            GroupTypeId = teamGroupType.Id,
            OwnerId = userId,
            Name = groupName,
            Description = description,
            StateFlag = StateFlags.ACTIVE
        };

        await _groupRepository.AddAsync(group);

        var membershipState = new GroupMembershipState
        {
            Id = Guid.NewGuid(),
            State = MembershipState.Active
        };
        await _groupMembershipStateRepository.AddAsync(membershipState);

        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = user.Id,
            MembershipState = membershipState,
            StateFlag = StateFlags.ACTIVE
        };

        await _groupMemberRepository.AddAsync(member);

        return group;
    }

    /// <summary>
    /// Adds a new user to the group with 'Pending' state, then moderates to 'Active'.
    /// </summary>
    public async Task<GroupMember> AddMemberToGroupAsync(Guid groupId, Guid userId, Guid? moderatorUserId = null, string? reason = null)
    {
        var pendingState = new GroupMembershipState
        {
            Id = Guid.NewGuid(),
            State = MembershipState.Pending
        };
        await _groupMembershipStateRepository.AddAsync(pendingState);

        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            UserId = userId,
            MembershipState = pendingState,
            StateFlag = StateFlags.ACTIVE
        };

        await _groupMemberRepository.AddAsync(member);

        await ModerateGroupMemberMembershipStateAsync(member.Id, MembershipState.Active, moderatorUserId, reason ?? DEFAULT_REASON_MEMBER_ADDED);

        return member;
    }

    /// <summary>
    /// Remove a group member by banning and marking as deleted.
    /// </summary>
    public async Task RemoveMemberFromGroupAsync(Guid groupMemberId, Guid? moderatorUserId = null, string? reason = null)
    {
        await ModerateGroupMemberMembershipStateAsync(groupMemberId, MembershipState.Banned, moderatorUserId, reason ?? DEFAULT_REASON_MEMBER_REMOVED);

        var member = await _groupMemberRepository.GetByIdAsync(groupMemberId)
            ?? throw new Exception("Group member not found");

        member.StateFlag = StateFlags.DELETED;
        await _groupMemberRepository.UpdateAsync(member);
    }

    /// <summary>
    /// Assign a role to a group member and audit the change.
    /// </summary>
    public async Task AssignRoleToGroupMemberAsync(Guid groupMemberId, Guid roleId, Guid? moderatorUserId = null, string? reason = null)
    {
        var member = await _groupMemberRepository.GetByIdAsync(groupMemberId)
            ?? throw new Exception("Group member not found");

        member.RoleId = roleId;
        await _groupMemberRepository.UpdateAsync(member);

        var audit = new GroupMemberAudit
        {
            Id = Guid.NewGuid(),
            MemberId = member.Id,
            GroupId = member.GroupId,
            NewState = member.MembershipState.State,
            Reason = reason ?? $"Role {roleId} assigned",
            ModeratorUserId = moderatorUserId ?? Guid.Empty,
            ChangedAt = DateTime.UtcNow,
            StateFlag = StateFlags.ACTIVE
        };

        await _moderationAuditRepository.AddAsync(audit);
    }

    /// <summary>
    /// Remove a role from a group member and audit the change.
    /// </summary>
    public async Task RemoveRoleFromGroupMemberAsync(Guid groupMemberId, Guid? moderatorUserId = null, string? reason = null)
    {
        var member = await _groupMemberRepository.GetByIdAsync(groupMemberId)
            ?? throw new Exception("Group member not found");

        member.RoleId = Guid.Empty;
        await _groupMemberRepository.UpdateAsync(member);

        var audit = new GroupMemberAudit
        {
            Id = Guid.NewGuid(),
            MemberId = member.Id,
            GroupId = member.GroupId,
            NewState = member.MembershipState.State,
            Reason = reason ?? "Role removed",
            ModeratorUserId = moderatorUserId ?? Guid.Empty,
            ChangedAt = DateTime.UtcNow,
            StateFlag = StateFlags.ACTIVE
        };

        await _moderationAuditRepository.AddAsync(audit);
    }

    /// <summary>
    /// Moderates group member's state via state entity and records the audit.
    /// </summary>
    public async Task ModerateGroupMemberMembershipStateAsync(
        Guid groupMemberId,
        MembershipState newState,
        Guid? moderatorUserId = null,
        string? reason = null)
    {
        var member = await _groupMemberRepository.GetByIdAsync(groupMemberId)
            ?? throw new Exception("Group member not found");

        var membershipState = new GroupMembershipState
        {
            Id = Guid.NewGuid(),
            State = newState,
            ModeratedByUserId = moderatorUserId ?? Guid.Empty,
            SuspendedAt = newState == MembershipState.Suspended ? DateTime.UtcNow : null,
            SuspensionReason = newState == MembershipState.Suspended ? reason : null,
            BannedAt = newState == MembershipState.Banned ? DateTime.UtcNow : null,
            BanReason = newState == MembershipState.Banned ? reason : null
        };
        await _groupMembershipStateRepository.AddAsync(membershipState);

        member.MembershipState = membershipState;
        await _groupMemberRepository.UpdateAsync(member);

        var audit = new GroupMemberAudit
        {
            Id = Guid.NewGuid(),
            MemberId = member.Id,
            GroupId = member.GroupId,
            NewState = newState,
            Reason = reason,
            ModeratorUserId = moderatorUserId ?? Guid.Empty,
            ChangedAt = DateTime.UtcNow,
            StateFlag = StateFlags.ACTIVE
        };

        await _moderationAuditRepository.AddAsync(audit);
    }

    #endregion

    #region Roles

    // Add your Roles-related logic here, always following the updated model structure.
    // Example (skeleton):

    public async Task AddRoleToUserAsync(Guid userId, Guid roleId)
    {
        // Your logic for user role assignment using _userRoleRepository, _roleRepository, etc.
        // Make sure to use navigation properties and audits if required.
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        // Your logic for user role removal.
    }

    #endregion

    #region Permissions

    // Add your Permissions-related logic here (skeleton):

    public async Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId)
    {
        // Logic for assigning a permission to a role.
    }

    public async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
    {
        // Logic for removing a permission from a role.
    }

    #endregion

    #region Utility Methods (Optional)

    // You can add more domain logic and utility methods as needed.

    #endregion
}

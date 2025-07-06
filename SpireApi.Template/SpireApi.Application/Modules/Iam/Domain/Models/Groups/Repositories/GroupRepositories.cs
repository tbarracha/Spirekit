using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.Constants;

namespace SpireApi.Application.Modules.Iam.Repositories;

// <summary>
/// Repository for <see cref="Group"/> entities.
/// </summary>
public class GroupRepository : BaseIamEntityRepository<Group>
{
    public GroupRepository(BaseIamDbContext context) : base(context) { }

    /// <summary>
    /// Gets a group by its name (case-insensitive, not deleted).
    /// </summary>
    public async Task<Group?> GetByNameAsync(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            throw new ArgumentException("Group name cannot be null or empty.", nameof(groupName));

        return await Query()
            .Where(g => g.Name.ToLower() == groupName.ToLower() && g.StateFlag != StateFlags.DELETED)
            .FirstOrDefaultAsync();
    }
}

/// <summary>
/// Repository for <see cref="GroupType"/> entities.
/// </summary>
public class GroupTypeRepository : BaseIamEntityRepository<GroupType>
{
    public GroupTypeRepository(BaseIamDbContext context) : base(context) { }
}

/// <summary>
/// Repository for <see cref="GroupMembershipState"/> entities.
/// </summary>
public class GroupMembershipStateRepository : BaseIamEntityRepository<GroupMembershipState>
{
    public GroupMembershipStateRepository(BaseIamDbContext context) : base(context) { }
}

/// <summary>
/// Repository for <see cref="GroupMember"/> entities.
/// </summary>
public class GroupMemberRepository : BaseIamEntityRepository<GroupMember>
{
    public GroupMemberRepository(BaseIamDbContext context) : base(context) { }
}

/// <summary>
/// Repository for <see cref="GroupMemberAudit"/> entities.
/// </summary>
public class GroupMemberAuditRepository : BaseIamEntityRepository<GroupMemberAudit>
{
    public GroupMemberAuditRepository(BaseIamDbContext context) : base(context) { }

    /// <summary>
    /// Gets all audit records for a specific group member.
    /// </summary>
    public async Task<IReadOnlyList<GroupMemberAudit>> GetAuditsByMemberIdAsync(Guid groupMemberId)
    {
        return await Query()
            .Where(a => a.MemberId == groupMemberId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all audit records for all members in a specific group.
    /// </summary>
    public async Task<IReadOnlyList<GroupMemberAudit>> GetAuditsByGroupIdAsync(Guid groupId)
    {
        return await Query()
            .Where(a => a.GroupId == groupId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets the latest audit record for a specific group member.
    /// </summary>
    public async Task<GroupMemberAudit?> GetLatestAuditAsync(Guid groupMemberId)
    {
        return await Query()
            .Where(a => a.MemberId == groupMemberId)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
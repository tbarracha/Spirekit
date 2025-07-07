using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Repositories;

/// <summary>
/// Repository for <see cref="GroupMemberAudit"/> entities.
/// </summary>
public class GroupMemberAuditRepository : BaseGroupEntityRepository<GroupMemberAudit>
{
    public GroupMemberAuditRepository(BaseIamDbContext context) : base(context) { }

    /// <summary>
    /// Gets all audit records for a specific group member.
    /// </summary>
    public async Task<IReadOnlyList<GroupMemberAudit>> GetAuditsByMemberIdAsync(Guid groupMemberId)
    {
        return await Query()
            .Where(a => a.GroupMemberUserId == groupMemberId)
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
            .Where(a => a.GroupMemberUserId == groupMemberId)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
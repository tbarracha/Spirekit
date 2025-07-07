using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.Constants;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Repositories;

// <summary>
/// Repository for <see cref="Group"/> entities.
/// </summary>
public class GroupRepository : BaseGroupEntityRepository<Group>
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

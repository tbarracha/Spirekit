using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Repositories;

/// <summary>
/// Repository for <see cref="GroupMembershipState"/> entities.
/// </summary>
public class GroupMembershipStateRepository : BaseGroupEntityRepository<GroupMembershipState>
{
    public GroupMembershipStateRepository(BaseIamDbContext context) : base(context) { }
}

using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Repositories;

/// <summary>
/// Repository for <see cref="GroupType"/> entities.
/// </summary>
public class GroupTypeRepository : BaseGroupEntityRepository<GroupType>
{
    public GroupTypeRepository(BaseIamDbContext context) : base(context) { }
}

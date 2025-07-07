using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Repositories;

/// <summary>
/// Repository for <see cref="GroupMember"/> entities.
/// </summary>
public class GroupMemberRepository : BaseGroupEntityRepository<GroupMember>
{
    public GroupMemberRepository(BaseIamDbContext context) : base(context) { }
}

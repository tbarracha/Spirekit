using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;

public class GroupMemberRepository : BaseIamEntityRepository<GroupMember>
{
    public GroupMemberRepository(BaseIamDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

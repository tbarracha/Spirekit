using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;

public class GroupMemberRepository : GuidEntityByRepository<GroupMember>
{
    // Constructor for DI
    public GroupMemberRepository(GuidEntityDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

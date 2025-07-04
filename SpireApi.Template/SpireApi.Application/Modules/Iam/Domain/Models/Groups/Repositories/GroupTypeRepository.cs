using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;

public class GroupTypeRepository : GuidEntityByRepository<GroupType>
{
    // Constructor for DI
    public GroupTypeRepository(GuidEntityDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

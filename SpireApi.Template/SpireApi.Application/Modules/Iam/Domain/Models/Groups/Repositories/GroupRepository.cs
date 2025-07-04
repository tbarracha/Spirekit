using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;

public class GroupRepository : GuidEntityByRepository<Group>
{
    // Constructor for DI
    public GroupRepository(GuidEntityDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Roles.Repositories;

public class RoleRepository : GuidEntityByRepository<Role>
{
    // Constructor for DI
    public RoleRepository(GuidEntityDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

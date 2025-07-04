using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Roles.Repositories;

public class RolePermissionRepository : GuidEntityByRepository<RolePermission>
{
    // Constructor for DI
    public RolePermissionRepository(GuidEntityDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

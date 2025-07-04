using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Permissions.Repositories;

public class PermissionRepository : GuidEntityByRepository<Permission>
{
    // Constructor for DI
    public PermissionRepository(GuidEntityDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Roles.Repositories;

public class RolePermissionRepository : BaseIamEntityRepository<RolePermission>
{
    public RolePermissionRepository(BaseIamDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

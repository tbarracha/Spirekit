using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Permissions.Repositories;

public class PermissionRepository : BaseIamEntityRepository<Permission>
{
    public PermissionRepository(BaseIamDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Permissions.Repositories;

public class PermissionScopeRepository : GuidEntityByRepository<PermissionScope>
{
    // Constructor for DI
    public PermissionScopeRepository(GuidEntityDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

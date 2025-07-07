using SpireApi.Application.Modules.Iam.Domain.Permissions.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Permissions.Repositories;

public class PermissionScopeRepository : BaseIamEntityRepository<PermissionScope>
{
    public PermissionScopeRepository(BaseIamDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}

using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Permissions.Models;

public class Permission : BaseIamEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    // Link to a scope
    public Guid? PermissionScopeId { get; set; }
    public PermissionScope? Resource { get; set; }

    public List<RolePermission> RolePermissions { get; set; } = new();
}
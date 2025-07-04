
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Permissions;

public class Permission : GuidEntityBy
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    // Link to a scope
    public Guid? PermissionScopeId { get; set; }
    public PermissionScope? Resource { get; set; }

    public List<RolePermission> RolePermissions { get; set; } = new();
}
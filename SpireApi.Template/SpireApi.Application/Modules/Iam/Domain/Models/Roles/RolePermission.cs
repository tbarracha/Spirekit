
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Roles;

public class RolePermission : GuidEntityBy
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = default!;
}
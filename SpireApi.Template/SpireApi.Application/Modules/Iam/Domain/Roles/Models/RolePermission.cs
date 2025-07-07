using SpireApi.Application.Modules.Iam.Domain.Permissions.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Roles.Models;

public class RolePermission : BaseIamEntity
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = default!;
}
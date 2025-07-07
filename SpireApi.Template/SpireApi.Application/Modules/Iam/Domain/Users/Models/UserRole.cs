using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Users.Models;

public class UserRole : BaseIamEntity
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;
}
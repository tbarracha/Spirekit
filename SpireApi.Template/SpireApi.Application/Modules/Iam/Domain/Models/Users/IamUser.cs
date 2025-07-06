using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.JWT.Identity.Users;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Users;

public class IamUser : BaseIamEntity, IJwtUserIdentity
{
    public Guid AuthUserId { get; set; }

    public string Email { get; set; } = default!;
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
}

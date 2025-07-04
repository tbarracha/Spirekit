using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUsers;
using SpireApi.Application.Modules.Authentication.Infrastructure;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Authentication.Domain.Models.RefreshTokens;

public class RefreshToken : BaseAuthEntity
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }

    public Guid AuthUserId { get; set; }
    public AuthUser AuthUser { get; set; } = default!;

    public bool IsRevoked { get; set; } = false;
}

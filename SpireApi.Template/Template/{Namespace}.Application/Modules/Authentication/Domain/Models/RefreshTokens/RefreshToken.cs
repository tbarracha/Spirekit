using {Namespace}.Application.Modules.Authentication.Domain.Models.AuthUsers;
using {Namespace}.Application.Shared.Entities;

namespace {Namespace}.Application.Modules.Authentication.Domain.Models.RefreshTokens;

public class RefreshToken : GuidEntity
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }

    public Guid AuthUserId { get; set; }
    public AuthUser AuthUser { get; set; } = default!;

    public bool IsRevoked { get; set; } = false;
}


using SpireApi.Application.Modules.Authentication.Domain.AuthUsers.Models;
using SpireCore.API.EntityFramework.Entities;

namespace SpireApi.Application.Modules.Authentication.Domain.RefreshTokens.Models;

public class RefreshToken : BaseEntityClass
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }

    public Guid AuthUserId { get; set; }
    public AuthUser AuthUser { get; set; } = default!;

    public bool IsRevoked { get; set; } = false;
}

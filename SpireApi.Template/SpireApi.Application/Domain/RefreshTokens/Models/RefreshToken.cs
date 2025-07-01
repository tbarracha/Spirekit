using SpireApi.Application.Domain.AppUsers.Models;
using SpireCore.API.EntityFramework.Entities;

namespace SpireApi.Application.Domain.RefreshTokens.Models;

public class RefreshToken : BaseEntityClass
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }

    public Guid AppUserId { get; set; }
    public AppUser User { get; set; } = default!;

    public bool IsRevoked { get; set; } = false;
}

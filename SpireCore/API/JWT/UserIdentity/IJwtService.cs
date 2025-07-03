using System.Security.Claims;

namespace SpireCore.API.JWT.UserIdentity;

public interface IJwtService
{
    string GenerateJwt(IJwtUser user, IEnumerable<Claim>? extraClaims = null, int? expiresInMinutes = null);
    ClaimsPrincipal? ValidateJwt(string token);
    IJwtUser? GetUserFromClaims(ClaimsPrincipal claimsPrincipal);

    // Added utility methods:
    Guid? GetUserIdFromToken(string jwtToken);
    bool IsTokenValid(string jwtToken);
    bool IsExpired(string jwtToken);
}

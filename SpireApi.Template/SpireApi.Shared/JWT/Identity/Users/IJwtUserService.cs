using System.Security.Claims;

namespace SpireApi.Shared.JWT.Identity.Users;

public interface IJwtUserService
{
    string GenerateUserJwt(IJwtUserIdentity user, IEnumerable<Claim>? extraClaims = null, int? expiresInMinutes = null);
    ClaimsPrincipal? ValidateUserJwt(string token);
    IJwtUserIdentity? GetUserFromClaims(ClaimsPrincipal claimsPrincipal);

    // Added utility methods:
    Guid? GetUserIdFromToken(string jwtToken);
    bool IsUserTokenValid(string jwtToken);
    bool IsUserExpired(string jwtToken);
}

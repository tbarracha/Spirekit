using System.Security.Claims;

namespace {Namespace}.Shared.JWT.MicroServiceIdentity;

public interface IServiceAuthenticationService
{
    string GenerateJwt(IJwtServiceAuth service, IEnumerable<Claim>? extraClaims = null, int? expiresInMinutes = null);
    ClaimsPrincipal? ValidateJwt(string token);
    IJwtServiceAuth? GetServiceFromClaims(ClaimsPrincipal claimsPrincipal);

    Guid? GetServiceIdFromToken(string jwtToken);
    bool IsTokenValid(string jwtToken);
    bool IsExpired(string jwtToken);
}

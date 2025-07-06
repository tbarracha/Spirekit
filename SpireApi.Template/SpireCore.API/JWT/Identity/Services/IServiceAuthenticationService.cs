using System.Security.Claims;

namespace SpireCore.API.JWT.Identity.Services;

public interface IServiceAuthenticationService
{
    string GenerateServiceJwt(IJwtServiceIdentity service, IEnumerable<Claim>? extraClaims = null, int? expiresInMinutes = null);
    ClaimsPrincipal? ValidateServiceJwt(string token);
    IJwtServiceIdentity? GetServiceFromClaims(ClaimsPrincipal claimsPrincipal);

    Guid? GetServiceIdFromToken(string jwtToken);
    bool IsServiceTokenValid(string jwtToken);
    bool IsServiceExpired(string jwtToken);
}
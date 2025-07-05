using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUserIdentities;
using System.Security.Claims;

namespace SpireApi.Application.Modules.Authentication.Domain.Services;

public interface IAuthUserIdentityService
{
    Task<(string AccessToken, string RefreshToken)> RegisterAsync(string email, string password, string firstName, string lastName, string? username);
    Task<(string AccessToken, string RefreshToken)> LoginAsync(string identifier, string password);
    Task LogoutAsync(string refreshToken);

    Task<string> RefreshTokenAsync(string token);
    Task<AuthUserIdentity?> GetAuthIdentityByIdAsync(Guid id);
    Task<AuthUserIdentity?> GetCurrentUserAsync(ClaimsPrincipal user);
    Task<AuthUserIdentity?> GetUserByTokenAsync(string jwtToken);
}

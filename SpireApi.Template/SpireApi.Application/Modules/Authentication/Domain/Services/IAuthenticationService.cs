using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUsers;
using System.Security.Claims;

namespace SpireApi.Application.Modules.Authentication.Domain.Services;

public interface IAuthenticationService
{
    Task<(string AccessToken, string RefreshToken)> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<(string AccessToken, string RefreshToken)> LoginAsync(string identifier, string password);
    Task<string> RefreshTokenAsync(string token);
    Task<AuthUser?> GetByIdAsync(Guid id);
    Task<AuthUser?> GetCurrentUserAsync(ClaimsPrincipal user);
    Task<AuthUser?> GetUserByTokenAsync(string jwtToken);
}

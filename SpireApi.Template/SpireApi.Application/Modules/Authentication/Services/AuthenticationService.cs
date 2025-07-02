using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SpireApi.Application.Domain.AuthUsers.Models;
using SpireApi.Application.Domain.AuthAudit;
using SpireApi.Application.Domain.RefreshTokens.Models;
using SpireApi.Application.Domain.RefreshTokens.Repositories;
using SpireApi.Application.Infrastructure;
using SpireCore.API.Services;
using SpireCore.Events.Dispatcher;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpireApi.Application.Features.Authentication.Services;

public class AuthenticationService : IAuthenticationService, ITransientService
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly SignInManager<AuthUser> _signInManager;
    private readonly IConfiguration _config;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly RefreshTokenRepository _refreshRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly BaseAuthDbContext _dbContext;

    public AuthenticationService(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        IConfiguration config,
        IEventDispatcher eventDispatcher,
        RefreshTokenRepository refreshRepo,
        IHttpContextAccessor httpContextAccessor,
        BaseAuthDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _eventDispatcher = eventDispatcher;
        _refreshRepo = refreshRepo;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public async Task<(string AccessToken, string RefreshToken)> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        var user = new AuthUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);

        await LogAuthAudit(user, AuthAuditType.Register, result.Succeeded, result.Succeeded ? null : string.Join(", ", result.Errors.Select(e => e.Description)));

        if (!result.Succeeded)
            throw new Exception("Registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        var accessToken = GenerateJwt(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return (accessToken, refreshToken);
    }

    public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string identifier, string password)
    {
        // Try to find by email first
        var user = await _userManager.FindByEmailAsync(identifier);

        // If not found by email, try by username
        user ??= await _userManager.FindByNameAsync(identifier);

        if (user == null)
        {
            await LogAuthAudit(null, AuthAuditType.Login, false, "User not found");
            throw new Exception("Invalid credentials");
        }

        // Sign in using the actual username (UserManager requires this)
        var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: false, lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            string failReason = signInResult.IsLockedOut ? "User locked out"
                                 : signInResult.IsNotAllowed ? "Login not allowed"
                                 : signInResult.RequiresTwoFactor ? "Requires two-factor"
                                 : "Invalid credentials";
            await LogAuthAudit(user, AuthAuditType.Login, false, failReason);
            throw new Exception(failReason);
        }

        // Update user login metadata
        user.UpdatedAt = DateTime.UtcNow;
        user.LastLoginAt = DateTime.UtcNow;
        user.LastLoginIp = GetIpAddress();
        user.LastLoginUserAgent = GetUserAgent();
        await _userManager.UpdateAsync(user);

        await LogAuthAudit(user, AuthAuditType.Login, true);

        var accessToken = GenerateJwt(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return (accessToken, refreshToken);
    }

    public async Task<string> RefreshTokenAsync(string token)
    {
        var record = await _refreshRepo.GetValidTokenAsync(token);
        if (record is null)
            throw new Exception("Invalid or expired refresh token.");

        await _refreshRepo.RevokeTokenAsync(record);
        return GenerateJwt(record.AuthUser!);
    }

    private string GenerateJwt(AuthUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("email", user.Email ?? string.Empty),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim("state", user.StateFlag ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateRefreshTokenAsync(AuthUser user)
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var expires = DateTime.UtcNow.AddDays(30);

        var refreshToken = new RefreshToken
        {
            Token = token,
            ExpiresAt = expires,
            AuthUserId = user.Id
        };

        await _refreshRepo.AddAsync(refreshToken);
        return token;
    }

    private async Task LogAuthAudit(AuthUser? user, string type, bool success, string? failureReason = null)
    {
        if (user == null) return;

        var audit = new AuthAudit
        {
            AuthUserId = user.Id,
            Type = type,
            WasSuccessful = success,
            IpAddress = GetIpAddress(),
            UserAgent = GetUserAgent(),
            FailureReason = failureReason
        };

        _dbContext.AuthAudits.Add(audit);
        await _dbContext.SaveChangesAsync();
    }

    private string? GetIpAddress()
        => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

    private string? GetUserAgent()
        => _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

    public async Task<AuthUser?> GetByIdAsync(Guid id)
        => await _userManager.FindByIdAsync(id.ToString());

    public async Task<AuthUser?> GetCurrentUserAsync(ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return id == null ? null : await _userManager.FindByIdAsync(id);
    }

    public async Task<AuthUser?> GetUserByTokenAsync(string jwtToken)
    {
        var userId = AuthTokenHelper.GetUserIdFromToken(jwtToken, _config);
        return userId.HasValue ? await _userManager.FindByIdAsync(userId.Value.ToString()) : null;
    }
}

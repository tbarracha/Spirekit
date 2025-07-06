using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SpireCore.Events.Dispatcher;
using System.Security.Claims;
using SpireApi.Application.Modules.Authentication.Infrastructure;
using SpireApi.Application.Modules.Authentication.Domain.Models.RefreshTokens;
using SpireApi.Application.Modules.Authentication.Domain.Models.AuthAudits;
using SpireCore.API.JWT.Identity.Users;
using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUserIdentities;
using SpireApi.Contracts.Events.Authentication;
using SpireCore.Services;

namespace SpireApi.Application.Modules.Authentication.Domain.Services;

public class AuthenticationService : IAuthUserIdentityService, ITransientService
{
    private readonly UserManager<AuthUserIdentity> _userManager;
    private readonly SignInManager<AuthUserIdentity> _signInManager;
    private readonly IConfiguration _config;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly RefreshTokenRepository _refreshRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly BaseAuthDbContext _dbContext;
    private readonly IJwtUserService _jwtService;

    public AuthenticationService(
        UserManager<AuthUserIdentity> userManager,
        SignInManager<AuthUserIdentity> signInManager,
        IConfiguration config,
        IEventDispatcher eventDispatcher,
        RefreshTokenRepository refreshRepo,
        IHttpContextAccessor httpContextAccessor,
        BaseAuthDbContext dbContext,
        IJwtUserService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _eventDispatcher = eventDispatcher;
        _refreshRepo = refreshRepo;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _jwtService = jwtService;
    }

    public async Task<(string AccessToken, string RefreshToken)> RegisterAsync(string email, string password, string firstName, string lastName, string? username = null)
    {
        var resolvedUsername = string.IsNullOrWhiteSpace(username) ? email : username;

        var user = new AuthUserIdentity
        {
            UserName = resolvedUsername,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);

        await LogAuthAudit(user, AuthAuditType.Register, result.Succeeded, result.Succeeded ? null : string.Join(", ", result.Errors.Select(e => e.Description)));

        if (!result.Succeeded)
            throw new Exception("Registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        var accessToken = _jwtService.GenerateUserJwt(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        await _eventDispatcher.PublishEventAsync(new AuthUserRegisteredEvent
        {
            AuthUserId = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            RegisteredAt = DateTime.UtcNow,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });

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

        var accessToken = _jwtService.GenerateUserJwt(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        // Dispatch LoggedIn event as side effect (optional)
        await _eventDispatcher.PublishEventAsync(new AuthUserLoggedInEvent
        {
            AuthUserId = user.Id,

            Email = user.Email!,
            UserName = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            LoggedInAt = DateTime.UtcNow,

            AccessToken = accessToken,
            RefreshToken = refreshToken
        });

        return (accessToken, refreshToken);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var record = await _refreshRepo.GetValidTokenAsync(refreshToken);
        if (record is not null)
        {
            await _refreshRepo.RevokeTokenAsync(record);
            await LogAuthAudit(record.AuthUser!, AuthAuditType.Logout, true);
        }
    }

    public async Task<string> RefreshTokenAsync(string token)
    {
        var record = await _refreshRepo.GetValidTokenAsync(token);
        if (record is null)
            throw new Exception("Invalid or expired refresh token.");

        await _refreshRepo.RevokeTokenAsync(record);
        return _jwtService.GenerateUserJwt(record.AuthUser!);
    }

    private async Task<string> GenerateRefreshTokenAsync(AuthUserIdentity user)
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

    private async Task LogAuthAudit(AuthUserIdentity? user, string type, bool success, string? failureReason = null)
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

    public async Task<AuthUserIdentity?> GetAuthIdentityByIdAsync(Guid id)
        => await _userManager.FindByIdAsync(id.ToString());

    public async Task<AuthUserIdentity?> GetCurrentUserAsync(ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        return id == null ? null : await _userManager.FindByIdAsync(id);
    }

    public async Task<AuthUserIdentity?> GetUserByTokenAsync(string jwtToken)
    {
        var userId = AuthTokenHelper.GetUserIdFromToken(jwtToken, _config);
        return userId.HasValue ? await _userManager.FindByIdAsync(userId.Value.ToString()) : null;
    }
}

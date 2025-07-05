using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUserIdentities;
using SpireApi.Application.Modules.Authentication.Domain.Models.RefreshTokens;
using SpireApi.Application.Modules.Authentication.Domain.Services;
using SpireApi.Application.Modules.Authentication.Infrastructure;

namespace SpireApi.Application.Modules.Authentication;

public static class AuthServiceExtensions
{
    /// <summary>
    /// Registers only domain authentication services (repositories, interfaces, custom logic).
    /// </summary>
    public static IServiceCollection AddAuthModuleServices(this IServiceCollection services)
    {
        // Identity registration (always added)
        services.AddIdentity<AuthUserIdentity, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<BaseAuthDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<RefreshTokenRepository>();
        services.AddScoped<IAuthUserIdentityService, AuthenticationService>();
        return services;
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using {Namespace}.Application.Modules.Authentication.Domain.Models.AuthUsers;
using {Namespace}.Application.Modules.Authentication.Domain.Models.RefreshTokens;
using {Namespace}.Application.Modules.Authentication.Domain.Services;
using {Namespace}.Application.Modules.Authentication.Infrastructure;

namespace {Namespace}.Application.Modules.Authentication.Configuration;

public static class AuthServiceExtensions
{
    /// <summary>
    /// Registers only domain authentication services (repositories, interfaces, custom logic).
    /// </summary>
    public static IServiceCollection AddDomainAuthServices(this IServiceCollection services)
    {
        // Identity registration (always added)
        services.AddIdentity<AuthUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<BaseAuthDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<RefreshTokenRepository>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        return services;
    }
}


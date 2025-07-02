using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SpireApi.Application.Domain.AuthUsers.Models;
using SpireApi.Application.Domain.RefreshTokens.Repositories;
using SpireApi.Application.Features.Authentication.Services;
using SpireApi.Application.Infrastructure;
using System.Text;

namespace SpireApi.Application.Features.Authentication;

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

    /// <summary>
    /// Registers .NET Identity, JWT authentication, and Authorization policies.
    /// </summary>
    public static IServiceCollection AddAspNetCoreAuth(this IServiceCollection services, IConfiguration configuration, string authSettingsSectionName = "AuthSettings")
    {
        var authSettings = configuration.GetSection(authSettingsSectionName).Get<AuthSettings>() ?? new AuthSettings();

        // --- Always add Authorization ---
        if (authSettings.Authentication)
        {
            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey) ||
                string.IsNullOrWhiteSpace(jwtIssuer) ||
                string.IsNullOrWhiteSpace(jwtAudience))
                throw new InvalidOperationException("Jwt:Key, Jwt:Issuer, or Jwt:Audience is not configured in appsettings.json");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            // Require authenticated user by default when enabled
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }
        else
        {
            // Always add Authorization, but with no policy—so all endpoints are open
            services.AddAuthorization();
        }

        return services;
    }
}

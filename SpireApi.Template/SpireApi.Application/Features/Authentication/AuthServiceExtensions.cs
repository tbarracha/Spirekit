using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SpireApi.Application.Domain.AppUsers.Models;
using SpireApi.Application.Domain.RefreshTokens.Repositories;
using SpireApi.Application.Persistance;
using SpireApi.Application.Features.Authentication.Services;

namespace SpireApi.Application.Features.Authentication;

public static class AuthServiceExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        // Identity for AuthUser/IdentityRole<Guid>
        services.AddIdentity<AuthUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<BaseAuthDbContext>()
            .AddDefaultTokenProviders();

        // --- Authentication Repositories ---
        services.AddScoped<RefreshTokenRepository>();

        // --- Authentication Services ---
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SpireCore.API.JWT.MicroServiceIdentity;
using SpireCore.Utils;
using System.Text;

namespace SpireCore.API.JWT;

public static class UnifiedJwtAuthExtensions
{
    /// <summary>
    /// Registers User and Service JWT authentication, the service identity, and the ServiceTokenProvider.
    /// </summary>
    public static IServiceCollection AddUnifiedJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Service Identity binding
        var serviceIdentitySection = configuration.GetSection("ServiceIdentity");
        var serviceIdentity = serviceIdentitySection.Get<ServiceIdentity>() ?? new ServiceIdentity();

        serviceIdentity.Id = GuidUtility.CreateDeterministicGuid(serviceIdentity.ServiceName);
        services.AddSingleton<IJwtServiceAuth>(serviceIdentity);
        services.AddSingleton<ServiceTokenProvider>();

        // 2. Get Jwt settings
        var jwtSection = configuration.GetSection("Jwt");
        var jwtKey = jwtSection["Key"];
        var jwtIssuer = jwtSection["Issuer"];
        var jwtAudience = jwtSection["Audience"];

        if (string.IsNullOrWhiteSpace(jwtKey) ||
            string.IsNullOrWhiteSpace(jwtIssuer) ||
            string.IsNullOrWhiteSpace(jwtAudience))
            throw new InvalidOperationException("Jwt:Key, Jwt:Issuer, or Jwt:Audience is not configured in appsettings.json");

        // 3. Add authentication and both schemes
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // "Bearer" for users
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            // USER JWT setup
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
        })
        .AddJwtBearer("ServiceBearer", options =>
        {
            // SERVICE JWT setup (could customize further if needed)
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

        // 4. Add Authorization policies (optional, but recommended)
        services.AddAuthorization(options =>
        {
            options.AddPolicy("UserJwt", policy =>
                policy.RequireAuthenticatedUser()
                      .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));

            options.AddPolicy("ServiceJwt", policy =>
                policy.RequireAuthenticatedUser()
                      .AddAuthenticationSchemes("ServiceBearer"));

            // Default policy (can point to user, or leave out for open API)
            options.DefaultPolicy = options.GetPolicy("UserJwt")!;
        });

        return services;
    }
}

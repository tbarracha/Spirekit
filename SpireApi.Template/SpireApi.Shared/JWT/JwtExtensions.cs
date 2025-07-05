using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SpireApi.Shared.JWT.Identity.Services;
using SpireApi.Shared.JWT.Identity.Users;
using SpireCore.Utils;
using System.Security.Claims;
using System.Text;

namespace SpireApi.Shared.JWT;

public static class JwtExtensions
{
    /// <summary>
    /// Registers User and Service JWT authentication, the service identity, and the ServiceTokenProvider.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Service Identity binding
        //var serviceIdentitySection = configuration.GetSection("ServiceIdentity");
        //var serviceIdentity = serviceIdentitySection.Get<ServiceIdentity>() ?? new ServiceIdentity();
        //
        //serviceIdentity.Id = GuidUtility.CreateDeterministicGuid(serviceIdentity.ServiceName);
        //services.AddSingleton<IJwtServiceIdentity>(serviceIdentity);
        //services.AddSingleton<ServiceTokenProvider>();

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
            // SERVICE JWT setup
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

        // 4. Add Authorization policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("UserJwt", policy =>
                policy.RequireAuthenticatedUser()
                      .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));

            options.AddPolicy("ServiceJwt", policy =>
                policy.RequireAuthenticatedUser()
                      .AddAuthenticationSchemes("ServiceBearer"));

            options.DefaultPolicy = options.GetPolicy("UserJwt")!;
        });

        return services;
    }

    /// <summary>
    /// Converts an IJwtUser to a set of JWT claims.
    /// </summary>
    public static IEnumerable<Claim> ToClaims(this IJwtUserIdentity user)
    {
        yield return new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());

        if (!string.IsNullOrEmpty(user.Email))
            yield return new Claim(ClaimTypes.Email, user.Email);

        if (!string.IsNullOrEmpty(user.UserName))
            yield return new Claim(ClaimTypes.Name, user.UserName);

        if (!string.IsNullOrEmpty(user.DisplayName))
            yield return new Claim("display_name", user.DisplayName);

        if (user is { FirstName: not null })
            yield return new Claim("first_name", user.FirstName!);

        if (user is { LastName: not null })
            yield return new Claim("last_name", user.LastName!);
    }
}

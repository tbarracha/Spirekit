using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpireApi.Application.Modules.Authentication.Domain.Services;

public static class AuthTokenHelper
{
    public static Guid? GetUserIdFromToken(string jwtToken, IConfiguration config)
    {
        var handler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
            ValidateLifetime = false // Skip lifetime validation here
        };

        try
        {
            var principal = handler.ValidateToken(jwtToken, validationParameters, out _);
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim is not null ? Guid.Parse(userIdClaim.Value) : null;
        }
        catch
        {
            return null;
        }
    }

    public static bool IsTokenValid(string jwtToken, IConfiguration config)
    {
        var handler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
        };

        try
        {
            handler.ValidateToken(jwtToken, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsExpired(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(jwtToken))
            return true;

        var token = handler.ReadJwtToken(jwtToken);
        return token.ValidTo < DateTime.UtcNow;
    }
}

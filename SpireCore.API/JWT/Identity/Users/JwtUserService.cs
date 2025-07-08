using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SpireCore.API.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpireCore.API.JWT.Identity.Users;

public class JwtUserService : IJwtUserService
{
    private readonly IConfiguration _config;

    public JwtUserService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateUserJwt(IJwtUserIdentity user, IEnumerable<Claim>? extraClaims = null, int? expiresInMinutes = null)
    {
        var claims = user.ToClaims().ToList();

        if (extraClaims != null)
            claims.AddRange(extraClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(expiresInMinutes ?? 60);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateUserJwt(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidAudience = _config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromMinutes(2)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public IJwtUserIdentity? GetUserFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null)
            return null;

        var idClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (idClaim == null || !Guid.TryParse(idClaim, out var id))
            return null;

        var email = claimsPrincipal.FindFirst("email")?.Value;
        var userName = claimsPrincipal.FindFirst("userName")?.Value;
        var displayName = claimsPrincipal.FindFirst("displayName")?.Value ?? userName;
        var firstName = claimsPrincipal.FindFirst("firstName")?.Value;
        var lastName = claimsPrincipal.FindFirst("lastName")?.Value;

        return new JwtUser
        {
            Id = id,
            UserName = userName,
            DisplayName = displayName,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };
    }



    // -------- Utility --------

    public Guid? GetUserIdFromToken(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidAudience = _config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
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

    public bool IsUserTokenValid(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidAudience = _config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!))
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

    public bool IsUserExpired(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(jwtToken))
            return true;

        var token = handler.ReadJwtToken(jwtToken);
        return token.ValidTo < DateTime.UtcNow;
    }
}

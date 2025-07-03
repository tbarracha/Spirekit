using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace {Namespace}.Shared.JWT.MicroServiceIdentity;

public class ServiceAuthenticationService : IServiceAuthenticationService
{
    private readonly IConfiguration _config;

    public ServiceAuthenticationService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateJwt(IJwtServiceAuth service, IEnumerable<Claim>? extraClaims = null, int? expiresInMinutes = null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, service.Id.ToString()),
            new Claim("serviceName", service.ServiceName ?? string.Empty)
        };

        if (!string.IsNullOrEmpty(service.Description))
            claims.Add(new Claim("description", service.Description));

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

    public ClaimsPrincipal? ValidateJwt(string token)
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

    public IJwtServiceAuth? GetServiceFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null)
            return null;

        var idClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (idClaim == null || !Guid.TryParse(idClaim, out var id))
            return null;

        var serviceName = claimsPrincipal.FindFirst("serviceName")?.Value;
        var description = claimsPrincipal.FindFirst("description")?.Value;

        return new JwtServiceAuth
        {
            Id = id,
            ServiceName = serviceName ?? string.Empty,
            Description = description
        };
    }

    public Guid? GetServiceIdFromToken(string jwtToken)
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
            var serviceIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return serviceIdClaim is not null ? Guid.Parse(serviceIdClaim.Value) : null;
        }
        catch
        {
            return null;
        }
    }

    public bool IsTokenValid(string jwtToken)
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

    public bool IsExpired(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(jwtToken))
            return true;

        var token = handler.ReadJwtToken(jwtToken);
        return token.ValidTo < DateTime.UtcNow;
    }
}

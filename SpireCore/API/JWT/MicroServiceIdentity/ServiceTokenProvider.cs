using System.IdentityModel.Tokens.Jwt;

namespace SpireCore.API.JWT.MicroServiceIdentity;

/// <summary>
/// Provides and caches a valid JWT for this microservice.
/// </summary>
public class ServiceTokenProvider
{
    private readonly IServiceAuthenticationService _jwtService;
    private readonly IJwtServiceAuth _serviceAuth;
    private string? _token;
    private DateTime _tokenExpiry;

    public ServiceTokenProvider(IServiceAuthenticationService jwtService, IJwtServiceAuth serviceAuth)
    {
        _jwtService = jwtService;
        _serviceAuth = serviceAuth;
        GenerateToken();
    }

    public string GetToken()
    {
        if (_token == null || DateTime.UtcNow > _tokenExpiry)
            GenerateToken();
        return _token!;
    }

    private void GenerateToken()
    {
        _token = _jwtService.GenerateJwt(_serviceAuth, expiresInMinutes: 60);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(_token);
        // Refresh 5 minutes before actual expiry
        _tokenExpiry = jwt.ValidTo.AddMinutes(-5);
    }
}

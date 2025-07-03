using {Namespace}.Application.Modules.Authentication.Domain.Services;
using {Namespace}.Contracts.Dtos.Modules.Authentication;

namespace {Namespace}.Application.Modules.Authentication.Operations;

public class RegisterOperation : AuthOperation<RegisterRequestDto, AuthResponseDto>
{
    public RegisterOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthResponseDto> ExecuteAsync(RegisterRequestDto request)
    {
        var (accessToken, refreshToken) = await _authenticationService.RegisterAsync(
            request.Email, request.Password, request.FirstName, request.LastName
        );
        return new AuthResponseDto { AccessToken = accessToken, RefreshToken = refreshToken };
    }
}


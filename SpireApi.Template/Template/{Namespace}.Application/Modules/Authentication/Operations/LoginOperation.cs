using {Namespace}.Application.Modules.Authentication.Domain.Services;
using {Namespace}.Contracts.Dtos.Modules.Authentication;

namespace {Namespace}.Application.Modules.Authentication.Operations;

public class LoginOperation : AuthOperation<LoginRequestDto, AuthResponseDto>
{
    public LoginOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthResponseDto> ExecuteAsync(LoginRequestDto request)
    {
        var (accessToken, refreshToken) = await _authenticationService.LoginAsync(
            request.Identifier, request.Password
        );
        return new AuthResponseDto { AccessToken = accessToken, RefreshToken = refreshToken };
    }
}


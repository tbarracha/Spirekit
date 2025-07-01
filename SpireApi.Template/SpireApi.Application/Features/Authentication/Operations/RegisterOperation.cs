using SpireApi.Application.Features.Authentication.Services;
using SpireApi.Contracts.Dtos.Authentication;

namespace SpireApi.Application.Features.Authentication.Operations;

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

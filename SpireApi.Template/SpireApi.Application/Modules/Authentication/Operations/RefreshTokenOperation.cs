using SpireApi.Application.Modules.Authentication.Domain.Services;
using SpireApi.Contracts.Dtos.Modules.Authentication;

namespace SpireApi.Application.Modules.Authentication.Operations;

public class RefreshTokenOperation : AuthOperation<RefreshTokenRequestDto, AuthResponseDto>
{
    public RefreshTokenOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthResponseDto> ExecuteAsync(RefreshTokenRequestDto request)
    {
        var accessToken = await _authenticationService.RefreshTokenAsync(request.RefreshToken);
        // If you want to also rotate the refresh token, update this as needed.
        return new AuthResponseDto { AccessToken = accessToken, RefreshToken = request.RefreshToken };
    }
}

using SpireApi.Application.Modules.Authentication.Domain.Services;

namespace SpireApi.Application.Modules.Authentication.Operations;

public class LogoutRequestDto
{
    public string RefreshToken { get; set; } = default!;
}

public class EmptyResponseDto { }

public class LogoutOperation : AuthOperation<LogoutRequestDto, EmptyResponseDto>
{
    public LogoutOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<EmptyResponseDto> ExecuteAsync(LogoutRequestDto request)
    {
        await _authenticationService.LogoutAsync(request.RefreshToken);
        return new EmptyResponseDto();
    }
}

using SpireApi.Application.Features.Authentication.Services;
using SpireApi.Application.Domain.AppUsers.Models;

namespace SpireApi.Application.Features.Authentication.Operations;

public class GetUserByTokenOperation : AuthOperation<string, AuthUser?>
{
    public GetUserByTokenOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthUser?> ExecuteAsync(string request)
    {
        return await _authenticationService.GetUserByTokenAsync(request);
    }
}

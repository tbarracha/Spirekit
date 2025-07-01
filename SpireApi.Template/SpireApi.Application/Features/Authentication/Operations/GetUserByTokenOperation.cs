using SpireApi.Application.Domain.AuthUsers.Models;
using SpireApi.Application.Features.Authentication.Services;
using SpireCore.API.Operations;

namespace SpireApi.Application.Features.Authentication.Operations;

[OperationAuthorize]
public class GetUserByTokenOperation : AuthOperation<string, AuthUser?>
{
    public GetUserByTokenOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthUser?> ExecuteAsync(string request)
    {
        return await _authenticationService.GetUserByTokenAsync(request);
    }
}

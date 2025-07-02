using SpireApi.Application.Modules.Authentication.Domain.AuthUsers.Models;
using SpireApi.Application.Modules.Authentication.Services;
using SpireCore.API.Operations;

namespace SpireApi.Application.Modules.Authentication.Operations;

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

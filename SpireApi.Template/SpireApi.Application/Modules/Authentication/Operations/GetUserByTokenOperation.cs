using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUsers;
using SpireApi.Application.Modules.Authentication.Domain.Services;
using SpireCore.API.Operations.Attributes;

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

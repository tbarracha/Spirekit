using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUserIdentities;
using SpireApi.Application.Modules.Authentication.Domain.Services;
using SpireApi.Shared.Operations.Attributes;

namespace SpireApi.Application.Modules.Authentication.Operations;

[OperationAuthorize]
public class GetUserByTokenOperation : AuthOperation<string, AuthUserIdentity?>
{
    public GetUserByTokenOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthUserIdentity?> ExecuteAsync(string request)
    {
        return await _authenticationService.GetUserByTokenAsync(request);
    }
}

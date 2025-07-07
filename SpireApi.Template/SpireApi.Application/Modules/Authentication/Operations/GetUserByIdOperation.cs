using SpireApi.Application.Modules.Authentication.Domain.AuthUserIdentities;
using SpireApi.Application.Modules.Authentication.Domain.Services;
using SpireCore.API.Operations.Attributes;

namespace SpireApi.Application.Modules.Authentication.Operations;

[OperationAuthorize]
public class GetUserByIdOperation : AuthOperation<Guid, AuthUserIdentity?>
{
    public GetUserByIdOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthUserIdentity?> ExecuteAsync(Guid request)
    {
        return await _authenticationService.GetAuthIdentityByIdAsync(request);
    }
}

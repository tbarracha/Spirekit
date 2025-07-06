using System.Security.Claims;
using SpireCore.API.Operations.Attributes;
using SpireApi.Application.Modules.Authentication.Domain.Services;
using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUserIdentities;

namespace SpireApi.Application.Modules.Authentication.Operations;

[OperationAuthorize]
public class GetCurrentUserOperation : AuthOperation<ClaimsPrincipal, AuthUserIdentity?>
{
    public GetCurrentUserOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthUserIdentity?> ExecuteAsync(ClaimsPrincipal request)
    {
        return await _authenticationService.GetCurrentUserAsync(request);
    }
}

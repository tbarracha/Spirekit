using System.Security.Claims;
using SpireCore.API.Operations;
using SpireApi.Application.Modules.Authentication.Domain.AuthUsers.Models;
using SpireApi.Application.Modules.Authentication.Services;

namespace SpireApi.Application.Modules.Authentication.Operations;

[OperationAuthorize]
public class GetCurrentUserOperation : AuthOperation<ClaimsPrincipal, AuthUser?>
{
    public GetCurrentUserOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthUser?> ExecuteAsync(ClaimsPrincipal request)
    {
        return await _authenticationService.GetCurrentUserAsync(request);
    }
}

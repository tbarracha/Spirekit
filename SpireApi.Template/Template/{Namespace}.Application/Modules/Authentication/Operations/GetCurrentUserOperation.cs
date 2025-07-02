using System.Security.Claims;
using SpireCore.API.Operations;
using {Namespace}.Application.Modules.Authentication.Domain.AuthUsers.Models;
using {Namespace}.Application.Modules.Authentication.Services;

namespace {Namespace}.Application.Modules.Authentication.Operations;

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


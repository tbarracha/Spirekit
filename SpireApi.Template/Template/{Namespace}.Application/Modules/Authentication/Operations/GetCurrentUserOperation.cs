using System.Security.Claims;
using {Namespace}.Shared.Operations.Attributes;
using {Namespace}.Application.Modules.Authentication.Domain.Services;
using {Namespace}.Application.Modules.Authentication.Domain.Models.AuthUsers;

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


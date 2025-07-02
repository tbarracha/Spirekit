using SpireApi.Application.Features.Authentication.Services;
using System.Security.Claims;
using SpireCore.API.Operations;
using SpireApi.Application.Features.Authentication.Domain.AuthUsers.Models;

namespace SpireApi.Application.Features.Authentication.Operations;

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

using SpireApi.Application.Features.Authentication.Services;
using SpireApi.Application.Domain.AuthUsers.Models;
using System.Security.Claims;
using SpireCore.API.Operations;

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

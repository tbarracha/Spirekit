using SpireApi.Application.Features.Authentication.Services;
using SpireApi.Application.Domain.AppUsers.Models;
using System.Security.Claims;

namespace SpireApi.Application.Features.Authentication.Operations;

public class GetCurrentUserOperation : AuthOperation<ClaimsPrincipal, AuthUser?>
{
    public GetCurrentUserOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthUser?> ExecuteAsync(ClaimsPrincipal request)
    {
        return await _authenticationService.GetCurrentUserAsync(request);
    }
}

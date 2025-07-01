using SpireApi.Application.Features.Authentication.Services;
using SpireApi.Application.Domain.AppUsers.Models;

namespace SpireApi.Application.Features.Authentication.Operations;

public class GetUserByIdOperation : AuthOperation<Guid, AuthUser?>
{
    public GetUserByIdOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthUser?> ExecuteAsync(Guid request)
    {
        return await _authenticationService.GetByIdAsync(request);
    }
}

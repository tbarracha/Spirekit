using SpireApi.Application.Domain.AuthUsers.Models;
using SpireApi.Application.Features.Authentication.Services;
using SpireCore.API.Operations;

namespace SpireApi.Application.Features.Authentication.Operations;

[OperationAuthorize]
public class GetUserByIdOperation : AuthOperation<Guid, AuthUser?>
{
    public GetUserByIdOperation(AuthenticationService authenticationService)
        : base(authenticationService) { }

    public override async Task<AuthUser?> ExecuteAsync(Guid request)
    {
        return await _authenticationService.GetByIdAsync(request);
    }
}

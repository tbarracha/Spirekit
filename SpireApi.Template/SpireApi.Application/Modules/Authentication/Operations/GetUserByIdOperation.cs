using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUsers;
using SpireApi.Application.Modules.Authentication.Domain.Services;
using SpireApi.Shared.Operations.Attributes;

namespace SpireApi.Application.Modules.Authentication.Operations;

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

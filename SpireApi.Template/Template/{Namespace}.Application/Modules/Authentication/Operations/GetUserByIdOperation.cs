using {Namespace}.Application.Modules.Authentication.Domain.AuthUsers.Models;
using {Namespace}.Application.Modules.Authentication.Services;
using SpireCore.API.Operations;

namespace {Namespace}.Application.Modules.Authentication.Operations;

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


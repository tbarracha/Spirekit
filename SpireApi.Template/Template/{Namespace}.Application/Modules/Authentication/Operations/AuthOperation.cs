using {Namespace}.Application.Modules.Authentication.Services;
using SpireCore.API.Operations;

namespace {Namespace}.Application.Modules.Authentication.Operations;

public abstract class AuthOperation<TRequest, TResponse> : IOperation<TRequest, TResponse>
{
    protected readonly AuthenticationService _authenticationService;

    protected AuthOperation(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public abstract Task<TResponse> ExecuteAsync(TRequest request);
}


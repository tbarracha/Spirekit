using Microsoft.AspNetCore.Authorization;

namespace SpireCore.API.Operations;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class OperationAuthorizeAttribute : AuthorizeAttribute
{
    public OperationAuthorizeAttribute(string? policy = null)
    {
        Policy = policy;
    }
}

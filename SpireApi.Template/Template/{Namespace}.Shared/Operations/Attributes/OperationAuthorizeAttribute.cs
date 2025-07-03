using Microsoft.AspNetCore.Authorization;

namespace {Namespace}.Shared.Operations.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class OperationAuthorizeAttribute : AuthorizeAttribute
{
    public OperationAuthorizeAttribute(string? policy = null)
    {
        Policy = policy;
    }
}


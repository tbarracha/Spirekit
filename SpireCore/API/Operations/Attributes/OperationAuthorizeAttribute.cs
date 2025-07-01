namespace SpireCore.API.Operations;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class OperationAuthorizeAttribute : Attribute
{
    public string? Policy { get; }

    public OperationAuthorizeAttribute(string? policy = null)
    {
        Policy = policy;
    }
}

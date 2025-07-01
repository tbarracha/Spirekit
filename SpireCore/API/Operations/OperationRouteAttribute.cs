namespace SpireCore.API.Operations;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class OperationRouteAttribute : Attribute
{
    public string Route { get; }

    public OperationRouteAttribute(string route)
    {
        Route = route.Trim().TrimStart('/');
    }
}

namespace SpireCore.API.Operations;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class OperationGroupAttribute : Attribute
{
    public string GroupName { get; }

    public OperationGroupAttribute(string groupName)
    {
        GroupName = groupName;
    }
}

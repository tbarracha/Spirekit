using System.Reflection;

namespace SpireApi.Shared.Operations.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class OperationGroupAttribute : Attribute
{
    public string GroupName { get; }

    public OperationGroupAttribute(string groupName)
    {
        GroupName = groupName;
    }

    public static string GetGroupName(Type opType)
    {
        // Try attribute first
        var groupAttr = opType.GetCustomAttribute<OperationGroupAttribute>();
        if (groupAttr != null && !string.IsNullOrWhiteSpace(groupAttr.GroupName))
            return groupAttr.GroupName;

        // Then try to extract group from namespace
        var ns = opType.Namespace;
        if (!string.IsNullOrWhiteSpace(ns))
        {
            var parts = ns.Split('.');
            var idx = Array.IndexOf(parts, "Operations");

            if (idx >= 0)
            {
                // If "Operations" is last, use the previous segment
                if (idx == parts.Length - 1 && idx > 0)
                    return parts[idx - 1];
                // If "Operations" has a group after, use the next segment
                if (idx < parts.Length - 1)
                    return parts[idx + 1];
            }
        }

        // Fallback
        return "misc";
    }
}

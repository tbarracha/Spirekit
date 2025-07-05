namespace SpireCore.Attributes.NormalizeFrom;

using System;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class NormalizedFromAttribute : Attribute
{
    public string SourceProperty { get; }

    public NormalizedFromAttribute(string sourceProperty)
    {
        SourceProperty = sourceProperty;
    }
}

using System.Reflection;

namespace SpireCore.Attributes.NormalizeFrom;

public static class NormalizationHelper
{
    public static void ApplyNormalization(object entity)
    {
        var type = entity.GetType();
        foreach (var targetProp in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attr = targetProp.GetCustomAttribute<NormalizedFromAttribute>();
            if (attr != null)
            {
                var sourceProp = type.GetProperty(attr.SourceProperty);
                if (sourceProp != null && sourceProp.PropertyType == typeof(string))
                {
                    var sourceValue = (string?)sourceProp.GetValue(entity);
                    targetProp.SetValue(entity, sourceValue?.ToUpperInvariant());
                }
            }
        }
    }
}

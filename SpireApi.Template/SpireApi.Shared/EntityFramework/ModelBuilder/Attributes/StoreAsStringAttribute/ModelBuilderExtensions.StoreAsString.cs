using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace SpireApi.Shared.EntityFramework.ModelBuilders.Attributes.StoreAsStringAttribute;

public static partial class ModelBuilderExtensions
{
    public static void ApplyEnumStringConversions(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null) continue;

            foreach (var prop in clrType.GetProperties())
            {
                var storeAsStringAttr = prop.GetCustomAttribute<StoreAsStringAttribute>(inherit: true);
                if (storeAsStringAttr is null) continue;

                if (prop.PropertyType.IsEnum)
                {
                    modelBuilder.Entity(clrType)
                        .Property(prop.Name)
                        .HasConversion<string>();
                }
            }
        }
    }
}

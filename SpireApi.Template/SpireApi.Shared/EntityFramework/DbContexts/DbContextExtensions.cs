using Microsoft.EntityFrameworkCore;
using SpireApi.Shared.EntityFramework.Entities.Base;
using System.Reflection;

namespace SpireApi.Shared.EntityFramework.DbContexts;

public static class DbContextExtensions
{
    /// <summary>
    /// Configure all enums to be stored as strings for this context.
    /// Call this from ConfigureConventions.
    /// </summary>
    public static void ConfigureEnumStorageAsString(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }

    /// <summary>
    /// Calls ConfigureEntity for all IEntity<> implementations in the model.
    /// </summary>
    public static void ApplyIEntityConfiguration(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            // Find IEntity<TId>
            var ientityInterface = clrType
                .GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>));

            if (ientityInterface != null)
            {
                var entityBuilder = modelBuilder.Entity(clrType);

                // Try to find the ConfigureEntity method
                var methodInfo = clrType.GetMethod(
                    "ConfigureEntity",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                if (methodInfo != null)
                {
                    var entityInstance = Activator.CreateInstance(clrType);
                    if (entityInstance != null)
                    {
                        methodInfo.Invoke(entityInstance, new object[] { entityBuilder });
                    }
                }
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace SpireApi.Application.Shared.Entities;

public class GuidEntityDbContext : DbContext
{
    public GuidEntityDbContext(DbContextOptions options)
        : base(options)
    {
    }

    // === Automatic entity configuration: ===
    // For all GuidEntity-derived types in this assembly, call their ConfigureEntity() method if present.
    // This enforces a convention-over-configuration approach.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply IEntityTypeConfiguration<T> (optional, if used)
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Automatically call ConfigureEntity for all GuidRepoEntity types
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Select(et => et.ClrType)
            .Where(clr =>
                (typeof(GuidEntity).IsAssignableFrom(clr) || typeof(GuidEntityBy).IsAssignableFrom(clr))
                && !clr.IsAbstract);

        foreach (var type in entityTypes)
        {
            var builder = modelBuilder.Entity(type);
            var configureMethod = type.GetMethod("ConfigureEntity",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (configureMethod != null)
            {
                // Create instance (assumes parameterless constructor)
                var instance = Activator.CreateInstance(type);
                configureMethod.Invoke(instance, new object[] { builder });
            }
        }
    }
}

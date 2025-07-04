using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireApi.Shared.EntityFramework.Entities.Abstractions;
using SpireCore.Abstractions.Interfaces;

namespace SpireApi.Shared.EntityFramework.Entities;

public static class BaseEntityConfigurationHelper
{
    // === Simple Properties ===

    public static void ConfigureHasId<T, TId>(EntityTypeBuilder<T> builder)
        where T : class, IHasId<TId>
    {
        builder.HasKey(nameof(IHasId<TId>.Id));
    }

    public static void ConfigureCreatedAt<T>(EntityTypeBuilder<T> builder)
        where T : class, ICreatedAt
    {
        builder.Property(nameof(ICreatedAt.CreatedAt)).IsRequired();
    }

    public static void ConfigureUpdatedAt<T>(EntityTypeBuilder<T> builder)
        where T : class, IUpdatedAt
    {
        builder.Property(nameof(IUpdatedAt.UpdatedAt)).IsRequired();
    }

    public static void ConfigureStateFlag<T>(EntityTypeBuilder<T> builder, int maxLength = 1)
        where T : class, IStateFlag
    {
        builder.Property(nameof(IStateFlag.StateFlag)).IsRequired().HasMaxLength(maxLength);
    }

    public static void ConfigureCreatedBy<T>(EntityTypeBuilder<T> builder, int maxLength = 100)
        where T : class, ICreatedBy
    {
        builder.Property(nameof(ICreatedBy.CreatedBy)).HasMaxLength(maxLength);
        builder.HasIndex(nameof(ICreatedBy.CreatedBy));
    }

    public static void ConfigureUpdatedBy<T>(EntityTypeBuilder<T> builder, int maxLength = 100)
        where T : class, IUpdatedBy
    {
        builder.Property(nameof(IUpdatedBy.UpdatedBy)).HasMaxLength(maxLength);
        builder.HasIndex(nameof(IUpdatedBy.UpdatedBy));
    }

    // === Composite Configurations for Your Domain Interfaces ===

    /// <summary>
    /// IEntity<TId> = IHasId<TId>, ICreatedAt, IUpdatedAt
    /// </summary>
    public static void ConfigureEntity<T, TId>(EntityTypeBuilder<T> builder)
        where T : class, IEntity<TId>
    {
        ConfigureHasId<T, TId>(builder);
        ConfigureCreatedAt(builder);
        ConfigureUpdatedAt(builder);
        ConfigureStateFlag(builder);
    }

    /// <summary>
    /// IAuditableEntity<TId> = IEntity<TId>, ICreatedBy, IUpdatedBy
    /// </summary>
    public static void ConfigureAuditableEntity<T, TId>(EntityTypeBuilder<T> builder)
        where T : class, IAuditableEntity<TId>
    {
        ConfigureEntity<T, TId>(builder);
        ConfigureCreatedBy(builder);
        ConfigureUpdatedBy(builder);
    }
}

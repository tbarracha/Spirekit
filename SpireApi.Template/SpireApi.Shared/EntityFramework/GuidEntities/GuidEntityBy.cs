using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireApi.Application.Shared.Entities;
using SpireCore.Abstractions.Interfaces;

/// <summary>
/// An auditable base entity with a Guid primary key, created/updated timestamps, state flag,
/// and user audit fields (<c>CreatedBy</c> and <c>UpdatedBy</c>).
/// Inherit from this class for entities that need to track the user responsible for creation or updates.
/// </summary>
public class GuidEntityBy : GuidEntity, ICreatedBy, IUpdatedBy
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Allows derived entities to customize their EF configuration.
    /// Call this from OnModelCreating in your DbContext.
    /// </summary>
    public virtual new void ConfigureEntity<T>(EntityTypeBuilder<T> builder) where T : GuidEntityBy
    {
        base.ConfigureEntity<T>(builder);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(100);

        builder.HasIndex(e => e.CreatedBy);
        builder.HasIndex(e => e.UpdatedBy);
    }
}

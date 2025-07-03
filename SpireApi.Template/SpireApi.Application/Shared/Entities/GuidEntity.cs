using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireCore.API.EntityFramework.Repositories;
using SpireCore.Constants;

namespace SpireApi.Application.Shared.Entities;

/// <summary>
/// A base entity with a Guid primary key, created/updated timestamps, and state flag.
/// Does NOT include audit fields like CreatedBy/UpdatedBy.
/// </summary>
public class GuidEntity : RepoEntity<Guid>
{
    public override Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Allows derived entities to customize their EF configuration.
    /// Call this from OnModelCreating in your DbContext.
    /// </summary>
    public virtual void ConfigureEntity<T>(EntityTypeBuilder<T> builder) where T : GuidEntity
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.StateFlag)
            .IsRequired()
            .HasDefaultValue(StateFlags.ACTIVE)
            .HasMaxLength(1);
    }
}

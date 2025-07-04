using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireApi.Shared.EntityFramework.Entities.Abstractions;
using SpireCore.Constants;

namespace SpireApi.Shared.EntityFramework.Entities.Implementations;

public abstract class BaseEntity<TId> : IEntity<TId>
{
    public TId Id { get; set; } = default!;

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string StateFlag { get; set; } = StateFlags.ACTIVE;


    // Generic virtual configuration for base fields
    public virtual void ConfigureEntity<T>(EntityTypeBuilder<T> builder) where T : BaseEntity<TId>
    {
        BaseEntityConfigurationHelper.ConfigureEntity<T, TId>(builder);
    }

}

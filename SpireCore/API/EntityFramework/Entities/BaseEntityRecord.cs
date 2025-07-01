using SpireCore.Abstractions.Interfaces;
using SpireCore.Constants;

namespace SpireCore.API.EntityFramework.Entities;

public abstract record BaseEntityRecord : ICreatedAt, IUpdatedAt, IStateFlag
{
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // State Flag
    public string StateFlag { get; set; } = StateFlags.ACTIVE;
}

public abstract record BaseEntityRecord<TId> : BaseEntityRecord, IHasId<TId>
{
    public TId Id { get; set; } = default!;
}

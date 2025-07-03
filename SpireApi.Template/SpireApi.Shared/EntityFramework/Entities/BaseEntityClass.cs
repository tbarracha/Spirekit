using SpireCore.Abstractions.Interfaces;
using SpireCore.Constants;

namespace SpireApi.Shared.EntityFramework.Entities;

public abstract class BaseEntityClass : ICreatedAt, IUpdatedAt, IStateFlag
{
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // State Flag
    public string StateFlag { get; set; } = StateFlags.ACTIVE;
}

public abstract class BaseEntityClass<TId> : BaseEntityClass, IHasId<TId>
{
    public TId Id { get; set; } = default!;
}

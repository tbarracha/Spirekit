
using SpireCore.Abstractions.Interfaces;

namespace SpireCore.API.EntityFramework.Entities;

public abstract class ChildEntity<TParent, TId, TParentId> : BaseEntityClass<TId>
    where TParent : IHasId<TParentId>
{
    // Foreign key
    public required TParentId ParentId { get; set; }

    // Navigation property
    public virtual TParent Parent { get; set; } = default!;
}


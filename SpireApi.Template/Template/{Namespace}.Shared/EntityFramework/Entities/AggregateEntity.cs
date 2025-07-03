namespace {Namespace}.Shared.EntityFramework.Entities;

public abstract class AggregateEntity<TId> : BaseEntityClass<TId>
{
    // Marker for aggregate roots.
    // Optionally, put domain events, versioning, or helper logic here.
}

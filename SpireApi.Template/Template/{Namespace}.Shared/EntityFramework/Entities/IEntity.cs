using SpireCore.Abstractions.Interfaces;

namespace {Namespace}.Shared.EntityFramework.Entities;

public interface IEntity<TId> : IHasId<TId>, ICreatedAt, IUpdatedAt
{

}


using SpireCore.Abstractions.Interfaces;

namespace {Namespace}.Shared.EntityFramework.Entities;

public interface IEntityWithState<TId> : IHasId<TId>, ICreatedAt, IUpdatedAt, IStateFlag
{

}


using SpireCore.Abstractions.Interfaces;

namespace SpireApi.Shared.EntityFramework.Entities.Abstractions;

public interface IEntity<TId> : IHasId<TId>, ICreatedAt, IUpdatedAt, IStateFlag
{

}

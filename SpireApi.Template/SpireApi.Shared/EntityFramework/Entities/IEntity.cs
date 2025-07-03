using SpireCore.Abstractions.Interfaces;

namespace SpireApi.Shared.EntityFramework.Entities;

public interface IEntity<TId> : IHasId<TId>, ICreatedAt, IUpdatedAt
{

}

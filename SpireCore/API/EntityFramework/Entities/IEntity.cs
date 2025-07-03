
using SpireCore.Abstractions.Interfaces;

namespace SpireCore.API.EntityFramework.Entities;

public interface IEntity<TId> : IHasId<TId>, ICreatedAt, IUpdatedAt
{

}

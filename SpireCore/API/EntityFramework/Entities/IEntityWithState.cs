
using SpireCore.Abstractions.Interfaces;

namespace SpireCore.API.EntityFramework.Entities;

public interface IEntityWithState<TId> : IHasId<TId>, ICreatedAt, IUpdatedAt, IStateFlag
{

}

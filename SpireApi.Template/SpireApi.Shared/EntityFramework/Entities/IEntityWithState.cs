using SpireCore.Abstractions.Interfaces;

namespace SpireApi.Shared.EntityFramework.Entities;

public interface IEntityWithState<TId> : IEntity<TId>, IStateFlag
{

}

using SpireCore.Abstractions.Interfaces;

namespace SpireApi.Shared.EntityFramework.Entities.Abstractions;

public interface IAuditableEntity<TId> : IEntity<TId>, ICreatedBy, IUpdatedBy, IStateFlag
{

}

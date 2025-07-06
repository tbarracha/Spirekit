using SpireCore.Abstractions.Interfaces;

namespace SpireApi.Shared.EntityFramework.Entities.Base;

public interface IAuditableEntity<TId> : IEntity<TId>, ICreatedBy, IUpdatedBy, IStateFlag
{

}

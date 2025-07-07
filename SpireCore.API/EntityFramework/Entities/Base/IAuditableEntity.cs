using SpireCore.Abstractions.Interfaces;

namespace SpireCore.API.EntityFramework.Entities.Base;

public interface IAuditableEntity<TId> : IEntity<TId>, ICreatedBy, IUpdatedBy, IStateFlag
{

}

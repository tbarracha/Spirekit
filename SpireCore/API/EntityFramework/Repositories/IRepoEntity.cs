// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// -----------------------------------------------------------------------------

using SpireCore.API.EntityFramework.Entities;

namespace SpireCore.API.EntityFramework.Repositories;

public interface IRepoEntity<TId> : IEntityWithState<TId>
{

}

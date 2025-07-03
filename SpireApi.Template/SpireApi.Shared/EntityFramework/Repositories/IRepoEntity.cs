
// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// -----------------------------------------------------------------------------

using SpireApi.Shared.EntityFramework.Entities;

namespace SpireApi.Shared.EntityFramework.Repositories;

public interface IRepoEntity<TId> : IEntityWithState<TId>
{

}

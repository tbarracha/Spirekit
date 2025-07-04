using SpireApi.Shared.EntityFramework.Repositories;

namespace SpireApi.Shared.Operations;

public abstract class CrudeOperation<T, TId, TContext, TRequest, TResponse> : IOperation<TRequest, TResponse>
    where T : class, IRepoEntity<TId>
    where TContext : Microsoft.EntityFrameworkCore.DbContext
{
    protected readonly IRepository<T, TId> _repository;

    // Inject the repository
    protected CrudeOperation(IRepository<T, TId> repository)
    {
        _repository = repository;
    }

    // The operation logic itself is abstract
    public abstract Task<TResponse> ExecuteAsync(TRequest request);
}

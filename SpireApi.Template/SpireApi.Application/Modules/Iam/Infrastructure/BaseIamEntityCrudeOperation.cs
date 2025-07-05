using SpireApi.Shared.Operations;
using SpireApi.Shared.Operations.Attributes;
using SpireCore.Services;

namespace SpireApi.Application.Modules.Iam.Infrastructure;

public abstract class BaseIamEntityCrudeOperation<TEntity, TRequest, TResponse> : IOperation<TRequest, TResponse>, ITransientService
    where TEntity : BaseIamEntity
{
    protected BaseIamEntityRepository<TEntity> _repository;

    public BaseIamEntityCrudeOperation(BaseIamEntityRepository<TEntity> entityRepository)
    {
        _repository = entityRepository;
    }

    public abstract Task<TResponse> ExecuteAsync(TRequest request);
}

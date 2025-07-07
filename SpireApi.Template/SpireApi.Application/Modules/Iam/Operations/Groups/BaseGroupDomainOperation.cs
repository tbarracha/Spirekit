using SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;
using SpireCore.API.Operations;
using SpireCore.API.Operations.Dtos;
using SpireCore.Services;

namespace SpireApi.Application.Modules.Iam.Operations.Groups;

/// <summary>
/// Base class for operations related to Group domain logic.
/// Provides access to Group repositories and services through GroupContext.
/// </summary>
public abstract class BaseGroupDomainOperation<TRequest, TResponse> : IOperation<AuditableRequestDto<TRequest>, TResponse>, ITransientService
{
    protected readonly GroupContext _groupContext;

    protected BaseGroupDomainOperation(GroupContext groupContext)
    {
        _groupContext = groupContext;
    }

    public abstract Task<TResponse> ExecuteAsync(AuditableRequestDto<TRequest> request);
}

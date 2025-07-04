// --------- BaseGroupCrudOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

/// <summary>
/// Abstract base class for all Group CRUD operations.
/// </summary>
public abstract class BaseGroupCrudOperation<TRequest, TResponse>
    : BaseIamEntityCrudeOperation<Group, AuditableRequestDto<TRequest>, TResponse>
{
    protected BaseGroupCrudOperation(BaseIamEntityRepository<Group> entityRepository)
        : base(entityRepository)
    { }
}

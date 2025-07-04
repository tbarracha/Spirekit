// --------- BaseGroupTypeCrudOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupTypeOperations;

/// <summary>
/// Abstract base class for all Group Type CRUD operations.
/// </summary>
public abstract class BaseGroupTypeCrudOperation<TRequest, TResponse>
    : BaseIamEntityCrudeOperation<GroupType, AuditableRequestDto<TRequest>, TResponse>
{
    protected BaseGroupTypeCrudOperation(BaseIamEntityRepository<GroupType> entityRepository)
        : base(entityRepository)
    { }
}

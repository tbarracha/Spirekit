// --------- BaseRoleCrudOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;

/// <summary>
/// Abstract base class for all Role CRUD operations.
/// </summary>
public abstract class BaseRoleCrudOperation<TRequest, TResponse>
    : BaseIamEntityCrudeOperation<Role, AuditableRequestDto<TRequest>, TResponse>
{
    protected BaseRoleCrudOperation(BaseIamEntityRepository<Role> entityRepository)
        : base(entityRepository)
    { }
}

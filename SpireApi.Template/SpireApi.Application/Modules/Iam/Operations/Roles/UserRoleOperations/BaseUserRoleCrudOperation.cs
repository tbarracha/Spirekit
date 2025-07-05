// --------- BaseRoleCrudOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.UserRoleOperations;

/// <summary>
/// Abstract base class for all Role CRUD operations.
/// </summary>
public abstract class BaseUserRoleCrudOperation<TRequest, TResponse>
    : BaseIamEntityCrudeOperation<UserRole, AuditableRequestDto<TRequest>, TResponse>
{
    protected BaseUserRoleCrudOperation(BaseIamEntityRepository<UserRole> entityRepository)
        : base(entityRepository)
    { }
}

// --------- BaseRolePermissionCrudOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RolePermissionOperations;

/// <summary>
/// Abstract base class for all Role Permission CRUD operations.
/// </summary>
public abstract class BaseRolePermissionCrudOperation<TRequest, TResponse>
    : BaseIamEntityCrudeOperation<RolePermission, AuditableRequestDto<TRequest>, TResponse>
{
    protected BaseRolePermissionCrudOperation(BaseIamEntityRepository<RolePermission> entityRepository)
        : base(entityRepository)
    { }
}

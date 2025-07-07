// --------- BaseRolePermissionCrudOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RolePermissionOperations;

/// <summary>
/// Abstract base class for all Role Permissions CRUD operations.
/// </summary>
public abstract class BaseRolePermissionCrudOperation<TRequest, TResponse>
    : BaseIamEntityCrudeOperation<RolePermission, AuditableRequestDto<TRequest>, TResponse>
{
    protected BaseRolePermissionCrudOperation(BaseIamEntityRepository<RolePermission> entityRepository)
        : base(entityRepository)
    { }
}

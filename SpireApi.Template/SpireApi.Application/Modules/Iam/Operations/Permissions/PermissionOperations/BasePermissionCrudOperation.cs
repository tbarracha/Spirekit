// --------- BasePermissionCrudOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionOperations;

/// <summary>
/// Abstract base class for all Permission CRUD operations.
/// </summary>
public abstract class BasePermissionCrudOperation<TRequest, TResponse>
    : BaseIamEntityCrudeOperation<Permission, AuditableRequestDto<TRequest>, TResponse>
{
    protected BasePermissionCrudOperation(BaseIamEntityRepository<Permission> entityRepository)
        : base(entityRepository)
    { }
}

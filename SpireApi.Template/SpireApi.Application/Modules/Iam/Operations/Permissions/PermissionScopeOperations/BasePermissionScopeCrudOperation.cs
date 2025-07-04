// --------- BasePermissionScopeCrudOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionScopeOperations;

/// <summary>
/// Abstract base class for all Permission Scope CRUD operations.
/// </summary>
public abstract class BasePermissionScopeCrudOperation<TRequest, TResponse>
    : BaseIamEntityCrudeOperation<PermissionScope, AuditableRequestDto<TRequest>, TResponse>
{
    protected BasePermissionScopeCrudOperation(BaseIamEntityRepository<PermissionScope> entityRepository)
        : base(entityRepository)
    { }
}

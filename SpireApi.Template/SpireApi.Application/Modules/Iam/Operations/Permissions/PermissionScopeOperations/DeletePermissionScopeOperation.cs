// --------- DeletePermissionScopeOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Permissions.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionScopeOperations;

public class DeletePermissionScopeDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Permission Scopes")]
[OperationRoute("permission/scopes/delete")]
public class DeletePermissionScopeOperation
    : BasePermissionScopeCrudOperation<DeletePermissionScopeDto, bool>
{
    public DeletePermissionScopeOperation(BaseIamEntityRepository<PermissionScope> repository)
        : base(repository) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeletePermissionScopeDto> request)
    {
        var entity = await _repository.GetByIdAsync(request.Data.Id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }
}

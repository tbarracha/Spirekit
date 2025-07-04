// --------- DeletePermissionScopeOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionScopeOperations;

public class DeletePermissionScopeDto
{
    public Guid Id { get; set; }
}

[OperationGroup("Permission Scope")]
[OperationRoute("permission-scope/delete")]
public class DeletePermissionScopeOperation
    : BasePermissionScopeCrudOperation<DeletePermissionScopeDto, bool>
{
    public DeletePermissionScopeOperation(BaseIamEntityRepository<PermissionScope> repository)
        : base(repository) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeletePermissionScopeDto> request)
    {
        var entity = await _repository.GetByIdAsync(request.data.Id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }
}

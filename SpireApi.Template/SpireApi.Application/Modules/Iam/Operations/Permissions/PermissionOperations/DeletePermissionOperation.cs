// --------- DeletePermissionOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionOperations;

public class DeletePermissionDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Permissions")]
[OperationRoute("permission/delete")]
public class DeletePermissionOperation
    : BasePermissionCrudOperation<DeletePermissionDto, bool>
{
    public DeletePermissionOperation(BaseIamEntityRepository<Permission> repository)
        : base(repository) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeletePermissionDto> request)
    {
        var entity = await _repository.GetByIdAsync(request.Data.Id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }
}

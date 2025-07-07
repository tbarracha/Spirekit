// --------- DeleteRolePermissionOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RolePermissionOperations;

public class DeleteRolePermissionDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Role Permissions")]
[OperationRoute("role/permissions/delete")]
public class DeleteRolePermissionOperation : BaseRolePermissionCrudOperation<DeleteRolePermissionDto, bool>
{
    public DeleteRolePermissionOperation(BaseIamEntityRepository<RolePermission> repository) : base(repository) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeleteRolePermissionDto> request)
    {
        var entity = await _repository.GetByIdAsync(request.Data.Id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }
}

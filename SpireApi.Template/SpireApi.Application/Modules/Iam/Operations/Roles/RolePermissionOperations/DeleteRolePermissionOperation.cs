// --------- DeleteRolePermissionOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RolePermissionOperations;

public class DeleteRolePermissionDto
{
    public Guid Id { get; set; }
}

[OperationGroup("Role Permission")]
[OperationRoute("role-permission/delete")]
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

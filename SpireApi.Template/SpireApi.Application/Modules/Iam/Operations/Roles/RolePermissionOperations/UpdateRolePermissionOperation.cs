// --------- UpdateRolePermissionOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RolePermissionOperations;

public class UpdateRolePermissionDto
{
    public Guid Id { get; set; }
    public Guid? RoleId { get; set; }
    public Guid? PermissionId { get; set; }
}

[OperationGroup("Role Permission")]
[OperationRoute("role-permission/update")]
public class UpdateRolePermissionOperation : BaseRolePermissionCrudOperation<UpdateRolePermissionDto, RolePermission?>
{
    public UpdateRolePermissionOperation(BaseIamEntityRepository<RolePermission> repository) : base(repository) { }

    public override async Task<RolePermission?> ExecuteAsync(AuditableRequestDto<UpdateRolePermissionDto> request)
    {
        var dto = request.data;
        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity == null) return null;

        if (dto.RoleId.HasValue) entity.RoleId = dto.RoleId.Value;
        if (dto.PermissionId.HasValue) entity.PermissionId = dto.PermissionId.Value;

        await _repository.UpdateAsync(entity);
        return entity;
    }
}

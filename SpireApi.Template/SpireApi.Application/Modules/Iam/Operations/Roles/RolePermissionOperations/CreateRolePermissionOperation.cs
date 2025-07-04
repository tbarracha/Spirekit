// --------- CreateRolePermissionOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RolePermissionOperations;

public class CreateRolePermissionDto
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}

[OperationGroup("Role Permission")]
[OperationRoute("role-permission/create")]
public class CreateRolePermissionOperation : BaseRolePermissionCrudOperation<CreateRolePermissionDto, RolePermission>
{
    public CreateRolePermissionOperation(BaseIamEntityRepository<RolePermission> repository) : base(repository) { }

    public override async Task<RolePermission> ExecuteAsync(AuditableRequestDto<CreateRolePermissionDto> request)
    {
        var dto = request.Data;
        var entity = new RolePermission
        {
            RoleId = dto.RoleId,
            PermissionId = dto.PermissionId
        };

        await _repository.AddAsync(entity);
        return entity;
    }
}

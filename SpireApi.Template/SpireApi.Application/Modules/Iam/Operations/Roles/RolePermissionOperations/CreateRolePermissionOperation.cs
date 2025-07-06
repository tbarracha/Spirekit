// --------- CreateRolePermissionOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RolePermissionOperations;

public class CreateRolePermissionDto
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}

[OperationGroup("IAM Role Permissions")]
[OperationRoute("role/permissions/create")]
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

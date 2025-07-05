// --------- GetRolePermissionByIdOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RolePermissionOperations;

public class GetRolePermissionByIdDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Role Permissions")]
[OperationRoute("role/permissions/get")]
public class GetRolePermissionByIdOperation : BaseRolePermissionCrudOperation<GetRolePermissionByIdDto, RolePermission?>
{
    public GetRolePermissionByIdOperation(BaseIamEntityRepository<RolePermission> repository) : base(repository) { }

    public override async Task<RolePermission?> ExecuteAsync(AuditableRequestDto<GetRolePermissionByIdDto> request)
    {
        return await _repository.GetByIdAsync(request.Data.Id);
    }
}

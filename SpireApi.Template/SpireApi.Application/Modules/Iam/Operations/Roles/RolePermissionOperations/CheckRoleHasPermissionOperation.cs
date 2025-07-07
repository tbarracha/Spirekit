using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;

public class CheckRoleHasPermissionRequestDto
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}

[OperationGroup("IAM Role Permissions")]
[OperationRoute("roles/has-permission")]
public class CheckRoleHasPermissionOperation
    : BaseRoleCrudOperation<CheckRoleHasPermissionRequestDto, bool>
{
    private readonly BaseIamEntityRepository<RolePermission> _rolePermissionRepository;

    public CheckRoleHasPermissionOperation(
        BaseIamEntityRepository<Role> roleRepository,
        BaseIamEntityRepository<RolePermission> rolePermissionRepository)
        : base(roleRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
    }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<CheckRoleHasPermissionRequestDto> request)
    {
        var dto = request.Data;

        return await _rolePermissionRepository.Query()
            .AnyAsync(rp => rp.RoleId == dto.RoleId && rp.PermissionId == dto.PermissionId);
    }
}

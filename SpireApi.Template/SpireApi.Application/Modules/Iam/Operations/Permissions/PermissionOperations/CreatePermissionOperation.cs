// --------- CreatePermissionOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionOperations;

public class CreatePermissionDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid? PermissionScopeId { get; set; }
}

[OperationGroup("IAM Permissions")]
[OperationRoute("permission/create")]
public class CreatePermissionOperation
    : BasePermissionCrudOperation<CreatePermissionDto, Permission>
{
    public CreatePermissionOperation(BaseIamEntityRepository<Permission> repository)
        : base(repository) { }

    public override async Task<Permission> ExecuteAsync(AuditableRequestDto<CreatePermissionDto> request)
    {
        var dto = request.Data;
        var entity = new Permission
        {
            Name = dto.Name,
            Description = dto.Description,
            PermissionScopeId = dto.PermissionScopeId
        };

        await _repository.AddAsync(entity);
        return entity;
    }
}

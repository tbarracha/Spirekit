// --------- UpdatePermissionOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionOperations;

public class UpdatePermissionDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? PermissionScopeId { get; set; }
}

[OperationGroup("IAM Permissions")]
[OperationRoute("permission/update")]
public class UpdatePermissionOperation
    : BasePermissionCrudOperation<UpdatePermissionDto, Permission?>
{
    public UpdatePermissionOperation(BaseIamEntityRepository<Permission> repository)
        : base(repository) { }

    public override async Task<Permission?> ExecuteAsync(AuditableRequestDto<UpdatePermissionDto> request)
    {
        var dto = request.Data;
        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name)) entity.Name = dto.Name;
        if (dto.Description is not null) entity.Description = dto.Description;
        if (dto.PermissionScopeId.HasValue) entity.PermissionScopeId = dto.PermissionScopeId;

        await _repository.UpdateAsync(entity);
        return entity;
    }
}

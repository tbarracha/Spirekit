// --------- UpdatePermissionScopeOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Permissions.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionScopeOperations;

public class UpdatePermissionScopeDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

[OperationGroup("IAM Permission Scopes")]
[OperationRoute("permission/scopes/update")]
public class UpdatePermissionScopeOperation
    : BasePermissionScopeCrudOperation<UpdatePermissionScopeDto, PermissionScope?>
{
    public UpdatePermissionScopeOperation(BaseIamEntityRepository<PermissionScope> repository)
        : base(repository) { }

    public override async Task<PermissionScope?> ExecuteAsync(AuditableRequestDto<UpdatePermissionScopeDto> request)
    {
        var dto = request.Data;
        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name)) entity.Name = dto.Name;
        if (dto.Description is not null) entity.Description = dto.Description;

        await _repository.UpdateAsync(entity);
        return entity;
    }
}

// --------- CreatePermissionScopeOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionScopeOperations;

public class CreatePermissionScopeDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

[OperationGroup("Permission Scope")]
[OperationRoute("permission-scope/create")]
public class CreatePermissionScopeOperation
    : BasePermissionScopeCrudOperation<CreatePermissionScopeDto, PermissionScope>
{
    public CreatePermissionScopeOperation(BaseIamEntityRepository<PermissionScope> repository)
        : base(repository) { }

    public override async Task<PermissionScope> ExecuteAsync(AuditableRequestDto<CreatePermissionScopeDto> request)
    {
        var dto = request.data;
        var entity = new PermissionScope
        {
            Name = dto.Name,
            Description = dto.Description
        };

        await _repository.AddAsync(entity);
        return entity;
    }
}

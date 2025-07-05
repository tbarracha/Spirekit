// --------- GetPermissionScopeByIdOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionScopeOperations;

public class GetPermissionScopeByIdDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Permission Scopes")]
[OperationRoute("permission/scopes/get")]
public class GetPermissionScopeByIdOperation
    : BasePermissionScopeCrudOperation<GetPermissionScopeByIdDto, PermissionScope?>
{
    public GetPermissionScopeByIdOperation(BaseIamEntityRepository<PermissionScope> repository)
        : base(repository) { }

    public override async Task<PermissionScope?> ExecuteAsync(AuditableRequestDto<GetPermissionScopeByIdDto> request)
    {
        return await _repository.GetByIdAsync(request.Data.Id);
    }
}

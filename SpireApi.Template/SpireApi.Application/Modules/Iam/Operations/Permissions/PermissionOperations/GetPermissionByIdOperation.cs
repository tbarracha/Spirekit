// --------- GetPermissionByIdOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionOperations;

public class GetPermissionByIdDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Permissions")]
[OperationRoute("permission/get")]
public class GetPermissionByIdOperation
    : BasePermissionCrudOperation<GetPermissionByIdDto, Permission?>
{
    public GetPermissionByIdOperation(BaseIamEntityRepository<Permission> repository)
        : base(repository) { }

    public override async Task<Permission?> ExecuteAsync(AuditableRequestDto<GetPermissionByIdDto> request)
    {
        return await _repository.GetByIdAsync(request.Data.Id);
    }
}

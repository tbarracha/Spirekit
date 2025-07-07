// --------- GetRoleByIdOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;

public class GetRoleByIdDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Roles")]
[OperationRoute("roles/get")]
public class GetRoleByIdOperation : BaseRoleCrudOperation<GetRoleByIdDto, Role?>
{
    public GetRoleByIdOperation(BaseIamEntityRepository<Role> repository) : base(repository) { }

    public override async Task<Role?> ExecuteAsync(AuditableRequestDto<GetRoleByIdDto> request)
    {
        return await _repository.GetByIdAsync(request.Data.Id);
    }
}

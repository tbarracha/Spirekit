// --------- GetRoleByIdOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

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

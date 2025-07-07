// --------- DeleteRoleOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;

public class DeleteRoleDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Roles")]
[OperationRoute("roles/delete")]
public class DeleteRoleOperation : BaseRoleCrudOperation<DeleteRoleDto, bool>
{
    public DeleteRoleOperation(BaseIamEntityRepository<Role> repository) : base(repository) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeleteRoleDto> request)
    {
        var entity = await _repository.GetByIdAsync(request.Data.Id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }
}

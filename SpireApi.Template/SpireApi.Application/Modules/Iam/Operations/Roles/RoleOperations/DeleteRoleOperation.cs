// --------- DeleteRoleOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;

public class DeleteRoleDto
{
    public Guid Id { get; set; }
}

[OperationGroup("Role")]
[OperationRoute("role/delete")]
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

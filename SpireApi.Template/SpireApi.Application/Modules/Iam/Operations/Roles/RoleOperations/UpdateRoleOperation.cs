// --------- UpdateRoleOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;

public class UpdateRoleDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? AccountId { get; set; }
}

[OperationGroup("IAM Roles")]
[OperationRoute("roles/update")]
public class UpdateRoleOperation : BaseRoleCrudOperation<UpdateRoleDto, Role?>
{
    public UpdateRoleOperation(BaseIamEntityRepository<Role> repository) : base(repository) { }

    public override async Task<Role?> ExecuteAsync(AuditableRequestDto<UpdateRoleDto> request)
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

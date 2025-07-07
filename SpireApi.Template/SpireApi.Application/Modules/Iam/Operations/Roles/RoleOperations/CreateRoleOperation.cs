// --------- CreateRoleOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;

public class CreateRoleDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid AccountId { get; set; }
}

[OperationGroup("IAM Roles")]
[OperationRoute("roles/create")]
public class CreateRoleOperation : BaseRoleCrudOperation<CreateRoleDto, Role>
{
    public CreateRoleOperation(BaseIamEntityRepository<Role> repository) : base(repository) { }

    public override async Task<Role> ExecuteAsync(AuditableRequestDto<CreateRoleDto> request)
    {
        var dto = request.Data;
        var entity = new Role
        {
            Name = dto.Name,
            Description = dto.Description,
        };

        await _repository.AddAsync(entity);
        return entity;
    }
}

// --------- CreateRoleOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;

public class CreateRoleDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid AccountId { get; set; }
}

[OperationGroup("Role")]
[OperationRoute("role/create")]
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

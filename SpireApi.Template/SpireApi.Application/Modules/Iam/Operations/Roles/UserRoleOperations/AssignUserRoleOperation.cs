using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.UserRoleOperations;

public class AssignUserRoleDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

[OperationGroup("IAM User Roles")]
[OperationRoute("user/roles/assign")]
public class AssignUserRoleOperation : BaseUserRoleCrudOperation<AssignUserRoleDto, UserRole>
{
    public AssignUserRoleOperation(BaseIamEntityRepository<UserRole> repository) : base(repository) { }

    public override async Task<UserRole> ExecuteAsync(AuditableRequestDto<AssignUserRoleDto> request)
    {
        var dto = request.Data;

        var entity = new UserRole
        {
            UserId = dto.UserId,
            RoleId = dto.RoleId
        };

        await _repository.AddAsync(entity);
        return entity;
    }
}

using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.UserRoleOperations;

public class RemoveUserRoleDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

[OperationGroup("IAM User Roles")]
[OperationRoute("user/roles/remove")]
public class RemoveUserRoleOperation : BaseUserRoleCrudOperation<RemoveUserRoleDto, UserRole?>
{
    public RemoveUserRoleOperation(BaseIamEntityRepository<UserRole> repository)
        : base(repository) { }

    public override async Task<UserRole?> ExecuteAsync(AuditableRequestDto<RemoveUserRoleDto> request)
    {
        var dto = request.Data;

        var entity = await _repository.Query()
            .Where(x => x.UserId == dto.UserId && x.RoleId == dto.RoleId)
            .FirstOrDefaultAsync();

        if (entity != null)
        {
            await _repository.DeleteAsync(entity);
        }

        return entity;
    }
}

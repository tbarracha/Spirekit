// --------- UpdateGroupMemberOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class UpdateGroupMemberDto
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public Guid? RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime JoinedAt { get; set; }
}

[OperationGroup("Group Member")]
[OperationRoute("group-member/update")]
public class UpdateGroupMemberOperation
    : BaseGroupMemberCrudOperation<UpdateGroupMemberDto, GroupMember?>
{
    public UpdateGroupMemberOperation(BaseIamEntityRepository<GroupMember> repository) : base(repository) { }

    public override async Task<GroupMember?> ExecuteAsync(AuditableRequestDto<UpdateGroupMemberDto> request)
    {
        var dto = request.data;
        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity == null) return null;

        entity.GroupId = dto.GroupId;
        entity.UserId = dto.UserId;
        entity.RoleId = dto.RoleId;
        entity.IsActive = dto.IsActive;
        entity.JoinedAt = dto.JoinedAt;

        await _repository.UpdateAsync(entity);
        return entity;
    }
}

using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class UpdateGroupMemberDto
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public Guid? RoleId { get; set; }
    public DateTime JoinedAt { get; set; }
}

[OperationGroup("IAM Group Members")]
[OperationRoute("group/member/update")]
public class UpdateGroupMemberOperation
    : BaseGroupDomainOperation<UpdateGroupMemberDto, GroupMember?>
{
    public UpdateGroupMemberOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<GroupMember?> ExecuteAsync(AuditableRequestDto<UpdateGroupMemberDto> request)
    {
        var dto = request.Data;
        var entity = await _groupContext.RepositoryContext.GroupMemberRepository.GetByIdAsync(dto.Id);
        if (entity == null) return null;

        entity.GroupId = dto.GroupId;
        entity.UserId = dto.UserId;
        entity.RoleId = dto.RoleId ?? Guid.Empty;
        entity.JoinedAt = dto.JoinedAt;

        await _groupContext.RepositoryContext.GroupMemberRepository.UpdateAsync(entity);
        return entity;
    }
}

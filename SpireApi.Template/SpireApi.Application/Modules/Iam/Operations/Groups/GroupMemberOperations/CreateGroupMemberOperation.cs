// --------- CreateGroupMemberOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class CreateGroupMemberDto
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public Guid? RoleId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? JoinedAt { get; set; }
}

[OperationGroup("IAM Group Members")]
[OperationRoute("group/member/create")]
public class CreateGroupMemberOperation
    : BaseGroupMemberCrudOperation<CreateGroupMemberDto, GroupMember>
{
    public CreateGroupMemberOperation(BaseIamEntityRepository<GroupMember> repository) : base(repository) { }

    public override async Task<GroupMember> ExecuteAsync(AuditableRequestDto<CreateGroupMemberDto> request)
    {
        var dto = request.Data;
        var entity = new GroupMember
        {
            GroupId = dto.GroupId,
            UserId = dto.UserId,
            RoleId = dto.RoleId,
            IsActive = dto.IsActive,
            JoinedAt = dto.JoinedAt ?? DateTime.UtcNow
        };
        await _repository.AddAsync(entity);
        return entity;
    }
}

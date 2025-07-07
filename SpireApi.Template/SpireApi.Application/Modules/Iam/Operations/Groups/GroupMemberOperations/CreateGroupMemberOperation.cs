using SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireCore.API.EntityFramework.Entities.Memberships;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;
using SpireCore.Constants;

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
    : BaseGroupDomainOperation<CreateGroupMemberDto, GroupMember>
{
    public CreateGroupMemberOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<GroupMember> ExecuteAsync(AuditableRequestDto<CreateGroupMemberDto> request)
    {
        var dto = request.Data;

        var defaultState = await _groupContext.RepositoryContext
            .GroupMembershipStateRepository
            .FirstOrDefaultAsync(s => s.IsDefault && s.StateFlag == StateFlags.ACTIVE);

        var entity = new GroupMember
        {
            StateId = defaultState?.Id ?? Guid.Empty,
            CurrentState = MembershipState.Active,
            GroupId = dto.GroupId,
            UserId = dto.UserId,
            RoleId = dto.RoleId ?? Guid.Empty,
            JoinedAt = dto.JoinedAt ?? DateTime.UtcNow
        };

        await _groupContext.RepositoryContext.GroupMemberRepository.AddAsync(entity);
        return entity;
    }
}

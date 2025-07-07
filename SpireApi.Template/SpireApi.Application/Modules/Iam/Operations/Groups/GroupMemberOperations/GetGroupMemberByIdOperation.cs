using SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Operations.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class GetGroupMemberByIdDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Group Members")]
[OperationRoute("group/member/get")]
public class GetGroupMemberByIdOperation
    : BaseGroupDomainOperation<GetGroupMemberByIdDto, GroupMember?>
{
    public GetGroupMemberByIdOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<GroupMember?> ExecuteAsync(AuditableRequestDto<GetGroupMemberByIdDto> request)
    {
        return await _groupContext.RepositoryContext.GroupMemberRepository.GetByIdAsync(request.Data.Id);
    }
}

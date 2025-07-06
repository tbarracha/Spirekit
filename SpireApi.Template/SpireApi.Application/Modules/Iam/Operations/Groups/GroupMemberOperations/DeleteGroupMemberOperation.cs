using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class DeleteGroupMemberDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Group Members")]
[OperationRoute("group/member/delete")]
public class DeleteGroupMemberOperation
    : BaseGroupDomainOperation<DeleteGroupMemberDto, bool>
{
    public DeleteGroupMemberOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeleteGroupMemberDto> request)
    {
        var entity = await _groupContext.RepositoryContext.GroupMemberRepository.GetByIdAsync(request.Data.Id);
        if (entity == null) return false;
        await _groupContext.RepositoryContext.GroupMemberRepository.DeleteAsync(entity);
        return true;
    }
}

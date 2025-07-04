// --------- DeleteGroupMemberOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class DeleteGroupMemberDto
{
    public Guid Id { get; set; }
}

[OperationGroup("Group Member")]
[OperationRoute("group-member/delete")]
public class DeleteGroupMemberOperation
    : BaseGroupMemberCrudOperation<DeleteGroupMemberDto, bool>
{
    public DeleteGroupMemberOperation(BaseIamEntityRepository<GroupMember> repository) : base(repository) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeleteGroupMemberDto> request)
    {
        var entity = await _repository.GetByIdAsync(request.Data.Id);
        if (entity == null) return false;
        await _repository.DeleteAsync(entity);
        return true;
    }
}

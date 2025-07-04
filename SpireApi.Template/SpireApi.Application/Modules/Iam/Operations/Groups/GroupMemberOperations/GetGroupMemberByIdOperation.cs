// --------- GetGroupMemberByIdOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class GetGroupMemberByIdDto
{
    public Guid Id { get; set; }
}

[OperationGroup("Group Member")]
[OperationRoute("group-member/get")]
public class GetGroupMemberByIdOperation
    : BaseGroupMemberCrudOperation<GetGroupMemberByIdDto, GroupMember?>
{
    public GetGroupMemberByIdOperation(BaseIamEntityRepository<GroupMember> repository) : base(repository) { }

    public override async Task<GroupMember?> ExecuteAsync(AuditableRequestDto<GetGroupMemberByIdDto> request)
    {
        return await _repository.GetByIdAsync(request.data.Id);
    }
}

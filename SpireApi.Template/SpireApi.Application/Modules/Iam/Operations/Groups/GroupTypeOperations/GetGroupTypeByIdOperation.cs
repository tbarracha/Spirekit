using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;
using SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Operations.Groups;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupTypeOperations;

public class GetGroupTypeByIdDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Group Types")]
[OperationRoute("group-type/get")]
public class GetGroupTypeByIdOperation
    : BaseGroupDomainOperation<GetGroupTypeByIdDto, GroupType?>
{
    public GetGroupTypeByIdOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<GroupType?> ExecuteAsync(AuditableRequestDto<GetGroupTypeByIdDto> request)
    {
        return await _groupContext.RepositoryContext.GroupTypeRepository.GetByIdAsync(request.Data.Id);
    }
}

using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class GetGroupByIdDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Groups")]
[OperationRoute("groups/get")]
public class GetGroupByIdOperation : BaseGroupDomainOperation<GetGroupByIdDto, Group?>
{
    public GetGroupByIdOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<Group?> ExecuteAsync(AuditableRequestDto<GetGroupByIdDto> request)
    {
        return await _groupContext.RepositoryContext.GroupRepository.GetByIdAsync(request.Data.Id);
    }
}

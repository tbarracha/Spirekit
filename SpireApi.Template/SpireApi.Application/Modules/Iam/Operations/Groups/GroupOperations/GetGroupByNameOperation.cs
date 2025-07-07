using SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Operations.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class GetGroupByNameDto
{
    public string Name { get; set; } = default!;
}

[OperationGroup("IAM Groups")]
[OperationRoute("groups/by-name")]
public class GetGroupByNameOperation : BaseGroupDomainOperation<GetGroupByNameDto, Group?>
{
    public GetGroupByNameOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<Group?> ExecuteAsync(AuditableRequestDto<GetGroupByNameDto> request)
    {
        // Assuming GroupRepository has GetByNameAsync method
        return await _groupContext.RepositoryContext.GroupRepository.GetByNameAsync(request.Data.Name);
    }
}

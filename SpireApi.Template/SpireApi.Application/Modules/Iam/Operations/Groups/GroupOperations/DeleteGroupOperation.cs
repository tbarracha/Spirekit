using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class DeleteGroupDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Groups")]
[OperationRoute("groups/delete")]
public class DeleteGroupOperation : BaseGroupDomainOperation<DeleteGroupDto, bool>
{
    public DeleteGroupOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeleteGroupDto> request)
    {
        var repo = _groupContext.RepositoryContext.GroupRepository;
        var group = await repo.GetByIdAsync(request.Data.Id);
        if (group == null) return false;
        await repo.DeleteAsync(group);
        return true;
    }
}

// --------- DeleteGroupOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class DeleteGroupDto
{
    public Guid Id { get; set; }
}

[OperationGroup("Group")]
[OperationRoute("group/delete")]
public class DeleteGroupOperation
    : BaseGroupCrudOperation<DeleteGroupDto, bool>
{
    public DeleteGroupOperation(BaseIamEntityRepository<Group> repository) : base(repository) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeleteGroupDto> request)
    {
        var group = await _repository.GetByIdAsync(request.data.Id);
        if (group == null) return false;
        await _repository.DeleteAsync(group);
        return true;
    }
}

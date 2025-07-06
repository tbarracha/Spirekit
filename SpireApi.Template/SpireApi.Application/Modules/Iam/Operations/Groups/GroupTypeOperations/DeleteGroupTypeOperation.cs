using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupTypeOperations;

public class DeleteGroupTypeDto
{
    public Guid Id { get; set; }
}

[OperationGroup("IAM Group Types")]
[OperationRoute("group-type/delete")]
public class DeleteGroupTypeOperation
    : BaseGroupDomainOperation<DeleteGroupTypeDto, bool>
{
    public DeleteGroupTypeOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeleteGroupTypeDto> request)
    {
        var entity = await _groupContext.RepositoryContext.GroupTypeRepository.GetByIdAsync(request.Data.Id);
        if (entity == null) return false;
        await _groupContext.RepositoryContext.GroupTypeRepository.DeleteAsync(entity);
        return true;
    }
}

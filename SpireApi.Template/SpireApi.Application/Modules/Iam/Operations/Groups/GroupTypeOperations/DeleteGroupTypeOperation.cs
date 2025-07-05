// --------- DeleteGroupTypeOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
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
    : BaseGroupTypeCrudOperation<DeleteGroupTypeDto, bool>
{
    public DeleteGroupTypeOperation(BaseIamEntityRepository<GroupType> repository) : base(repository) { }

    public override async Task<bool> ExecuteAsync(AuditableRequestDto<DeleteGroupTypeDto> request)
    {
        var entity = await _repository.GetByIdAsync(request.Data.Id);
        if (entity == null) return false;
        await _repository.DeleteAsync(entity);
        return true;
    }
}

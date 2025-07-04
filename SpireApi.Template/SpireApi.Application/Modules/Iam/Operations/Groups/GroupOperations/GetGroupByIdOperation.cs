// --------- GetGroupByIdOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class GetGroupByIdDto
{
    public Guid Id { get; set; }
}

[OperationGroup("Group")]
[OperationRoute("group/get")]
public class GetGroupByIdOperation
    : BaseGroupCrudOperation<GetGroupByIdDto, Group?>
{
    public GetGroupByIdOperation(BaseIamEntityRepository<Group> repository) : base(repository) { }

    public override async Task<Group?> ExecuteAsync(AuditableRequestDto<GetGroupByIdDto> request)
    {
        return await _repository.GetByIdAsync(request.data.Id);
    }
}

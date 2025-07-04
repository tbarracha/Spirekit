// --------- GetGroupTypeByIdOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupTypeOperations;

public class GetGroupTypeByIdDto
{
    public Guid Id { get; set; }
}

[OperationGroup("Group Type")]
[OperationRoute("group-type/get")]
public class GetGroupTypeByIdOperation
    : BaseGroupTypeCrudOperation<GetGroupTypeByIdDto, GroupType?>
{
    public GetGroupTypeByIdOperation(BaseIamEntityRepository<GroupType> repository) : base(repository) { }

    public override async Task<GroupType?> ExecuteAsync(AuditableRequestDto<GetGroupTypeByIdDto> request)
    {
        return await _repository.GetByIdAsync(request.data.Id);
    }
}

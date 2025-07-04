// --------- CreateGroupTypeOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupTypeOperations;

public class CreateGroupTypeDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

[OperationGroup("Group Type")]
[OperationRoute("group-type/create")]
public class CreateGroupTypeOperation
    : BaseGroupTypeCrudOperation<CreateGroupTypeDto, GroupType>
{
    public CreateGroupTypeOperation(BaseIamEntityRepository<GroupType> repository) : base(repository) { }

    public override async Task<GroupType> ExecuteAsync(AuditableRequestDto<CreateGroupTypeDto> request)
    {
        var dto = request.data;
        var entity = new GroupType
        {
            Name = dto.Name,
            Description = dto.Description
        };
        await _repository.AddAsync(entity);
        return entity;
    }
}

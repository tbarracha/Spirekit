// --------- UpdateGroupTypeOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupTypeOperations;

public class UpdateGroupTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

[OperationGroup("Group Type")]
[OperationRoute("group-type/update")]
public class UpdateGroupTypeOperation
    : BaseGroupTypeCrudOperation<UpdateGroupTypeDto, GroupType?>
{
    public UpdateGroupTypeOperation(BaseIamEntityRepository<GroupType> repository) : base(repository) { }

    public override async Task<GroupType?> ExecuteAsync(AuditableRequestDto<UpdateGroupTypeDto> request)
    {
        var dto = request.data;
        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Description = dto.Description;

        await _repository.UpdateAsync(entity);
        return entity;
    }
}

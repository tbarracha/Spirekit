// --------- UpdateGroupOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class UpdateGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid OwnerUserId { get; set; }
    public Guid GroupTypeId { get; set; }
}

[OperationGroup("IAM Groups")]
[OperationRoute("groups/update")]
public class UpdateGroupOperation
    : BaseGroupCrudOperation<UpdateGroupDto, Group?>
{
    public UpdateGroupOperation(BaseIamEntityRepository<Group> repository) : base(repository) { }

    public override async Task<Group?> ExecuteAsync(AuditableRequestDto<UpdateGroupDto> request)
    {
        var dto = request.Data;
        var group = await _repository.GetByIdAsync(dto.Id);
        if (group == null) return null;

        group.Name = dto.Name;
        group.Description = dto.Description;
        group.OwnerUserId = dto.OwnerUserId;
        group.GroupTypeId = dto.GroupTypeId;

        await _repository.UpdateAsync(group);
        return group;
    }
}

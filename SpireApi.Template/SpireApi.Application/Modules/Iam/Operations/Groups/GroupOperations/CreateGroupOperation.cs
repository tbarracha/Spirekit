// --------- CreateGroupOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class CreateGroupDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid OwnerUserId { get; set; }
    public Guid GroupTypeId { get; set; }
}

[OperationGroup("IAM Groups")]
[OperationRoute("groups/create")]
public class CreateGroupOperation
    : BaseGroupCrudOperation<CreateGroupDto, Group>
{
    public CreateGroupOperation(BaseIamEntityRepository<Group> repository) : base(repository) { }

    public override async Task<Group> ExecuteAsync(AuditableRequestDto<CreateGroupDto> request)
    {
        var dto = request.Data;
        var group = new Group
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerUserId = dto.OwnerUserId,
            GroupTypeId = dto.GroupTypeId
        };
        await _repository.AddAsync(group);
        return group;
    }
}

using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class UpdateGroupDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = default!;
	public string? Description { get; set; }
	public Guid OwnerId { get; set; }
	public Guid GroupTypeId { get; set; }
}

[OperationGroup("IAM Groups")]
[OperationRoute("groups/update")]
public class UpdateGroupOperation : BaseGroupDomainOperation<UpdateGroupDto, Group?>
{
	public UpdateGroupOperation(GroupContext groupContext) : base(groupContext) { }

	public override async Task<Group?> ExecuteAsync(AuditableRequestDto<UpdateGroupDto> request)
	{
		var dto = request.Data;
		var repo = _groupContext.RepositoryContext.GroupRepository;
		var group = await repo.GetByIdAsync(dto.Id);
		if (group == null) return null;

		group.Name = dto.Name;
		group.Description = dto.Description;
		group.OwnerId = dto.OwnerId;
		group.GroupTypeId = dto.GroupTypeId;

		await repo.UpdateAsync(group);
		return group;
	}
}

using SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Operations.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class CreateGroupDto
{
	public string Name { get; set; } = default!;
	public string? Description { get; set; }
	public Guid OwnerId { get; set; }
	public Guid GroupTypeId { get; set; }
}

[OperationGroup("IAM Groups")]
[OperationRoute("groups/create")]
public class CreateGroupOperation : BaseGroupDomainOperation<CreateGroupDto, Group>
{
	public CreateGroupOperation(GroupContext groupContext) : base(groupContext) { }

	public override async Task<Group> ExecuteAsync(AuditableRequestDto<CreateGroupDto> request)
	{
		var dto = request.Data;
		var group = new Group
		{
			Name = dto.Name,
			Description = dto.Description,
			OwnerUserId = dto.OwnerId,
			GroupTypeId = dto.GroupTypeId
		};
		await _groupContext.RepositoryContext.GroupRepository.AddAsync(group);
		return group;
	}
}

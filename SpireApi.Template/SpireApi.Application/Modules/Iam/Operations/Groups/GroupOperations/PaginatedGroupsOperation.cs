using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class PaginatedGroupsRequestDto
{
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 20;
	public string? Name { get; set; }
	public Guid? OwnerId { get; set; }
	public Guid? GroupTypeId { get; set; }
	// Add other filters as needed
}

[OperationGroup("IAM Groups")]
[OperationRoute("groups/page")]
public class PaginatedGroupsOperation : BaseGroupDomainOperation<PaginatedGroupsRequestDto, PaginatedResult<Group>>
{
	public PaginatedGroupsOperation(GroupContext groupContext) : base(groupContext) { }

	public override async Task<PaginatedResult<Group>> ExecuteAsync(AuditableRequestDto<PaginatedGroupsRequestDto> request)
	{
		var filter = request.Data;
		var query = _groupContext.RepositoryContext.GroupRepository.Query();

		if (!string.IsNullOrWhiteSpace(filter.Name))
			query = query.Where(g => g.Name.Contains(filter.Name));
		if (filter.OwnerId.HasValue)
			query = query.Where(g => g.OwnerId == filter.OwnerId.Value);
		if (filter.GroupTypeId.HasValue)
			query = query.Where(g => g.GroupTypeId == filter.GroupTypeId.Value);

		var totalCount = await query.CountAsync();
		var items = await query
			.Skip((filter.Page - 1) * filter.PageSize)
			.Take(filter.PageSize)
			.ToListAsync();

		return new PaginatedResult<Group>(items, totalCount, filter.Page, filter.PageSize);
	}
}

using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class PaginatedGroupsByTypeRequestDto
{
	public Guid GroupTypeId { get; set; }
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 20;
	public string? Name { get; set; }
	public Guid? OwnerId { get; set; }
}

[OperationGroup("IAM Groups")]
[OperationRoute("groups/type/page")]
public class PaginatedGroupsByTypeOperation : BaseGroupDomainOperation<PaginatedGroupsByTypeRequestDto, PaginatedResult<Group>>
{
	public PaginatedGroupsByTypeOperation(GroupContext groupContext) : base(groupContext) { }

	public override async Task<PaginatedResult<Group>> ExecuteAsync(AuditableRequestDto<PaginatedGroupsByTypeRequestDto> request)
	{
		var filter = request.Data;
		var query = _groupContext.RepositoryContext.GroupRepository.Query()
			.Where(g => g.GroupTypeId == filter.GroupTypeId);

		if (!string.IsNullOrWhiteSpace(filter.Name))
			query = query.Where(g => g.Name.Contains(filter.Name));
		if (filter.OwnerId.HasValue)
			query = query.Where(g => g.OwnerId == filter.OwnerId.Value);

		var totalCount = await query.CountAsync();
		var items = await query
			.Skip((filter.Page - 1) * filter.PageSize)
			.Take(filter.PageSize)
			.ToListAsync();

		return new PaginatedResult<Group>(items, totalCount, filter.Page, filter.PageSize);
	}
}

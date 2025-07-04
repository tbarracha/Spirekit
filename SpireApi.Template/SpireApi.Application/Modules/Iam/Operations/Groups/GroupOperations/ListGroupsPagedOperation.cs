// --------- ListGroupsPagedOperation.cs ---------
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class ListGroupsPagedDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public string? Name { get; set; }
    public Guid? OwnerUserId { get; set; }
    public Guid? GroupTypeId { get; set; }
    // Add other filters as needed
}

[OperationGroup("Group")]
[OperationRoute("group/list")]
public class ListGroupsPagedOperation
    : BaseGroupCrudOperation<ListGroupsPagedDto, PaginatedResult<Group>>
{
    public ListGroupsPagedOperation(BaseIamEntityRepository<Group> repository) : base(repository) { }

    public override async Task<PaginatedResult<Group>> ExecuteAsync(AuditableRequestDto<ListGroupsPagedDto> request)
    {
        var filter = request.Data;
        var query = _repository.Query();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(g => g.Name.Contains(filter.Name));
        if (filter.OwnerUserId.HasValue)
            query = query.Where(g => g.OwnerUserId == filter.OwnerUserId.Value);
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

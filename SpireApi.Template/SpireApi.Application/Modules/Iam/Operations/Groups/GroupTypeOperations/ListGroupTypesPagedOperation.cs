// --------- ListGroupTypesPagedOperation.cs ---------
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupTypeOperations;

public class ListGroupTypesPagedDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public string? Name { get; set; }
    // Add more filters as needed
}

[OperationGroup("Group Type")]
[OperationRoute("group-type/list")]
public class ListGroupTypesPagedOperation
    : BaseGroupTypeCrudOperation<ListGroupTypesPagedDto, PaginatedResult<GroupType>>
{
    public ListGroupTypesPagedOperation(BaseIamEntityRepository<GroupType> repository) : base(repository) { }

    public override async Task<PaginatedResult<GroupType>> ExecuteAsync(AuditableRequestDto<ListGroupTypesPagedDto> request)
    {
        var filter = request.data;
        var query = _repository.Query();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(gt => gt.Name.Contains(filter.Name));

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
        return new PaginatedResult<GroupType>(items, totalCount, filter.Page, filter.PageSize);
    }
}

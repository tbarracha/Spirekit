using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupTypeOperations;

public class PaginatedGroupTypesRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Name { get; set; }
}

[OperationGroup("IAM Group Types")]
[OperationRoute("group-type/page")]
public class PaginatedGroupTypesOperation
    : BaseGroupDomainOperation<PaginatedGroupTypesRequestDto, PaginatedResult<GroupType>>
{
    public PaginatedGroupTypesOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<PaginatedResult<GroupType>> ExecuteAsync(AuditableRequestDto<PaginatedGroupTypesRequestDto> request)
    {
        var filter = request.Data;
        var query = _groupContext.RepositoryContext.GroupTypeRepository.Query();

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

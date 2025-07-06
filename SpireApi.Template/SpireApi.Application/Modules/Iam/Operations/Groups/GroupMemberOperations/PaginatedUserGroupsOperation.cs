using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class PaginatedUserGroupsRequestDto
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Name { get; set; }
    public Guid? OwnerId { get; set; }
    public Guid? GroupTypeId { get; set; }
}

[OperationGroup("IAM User Groups")]
[OperationRoute("user/groups/page")]
public class PaginatedUserGroupsOperation
    : BaseGroupDomainOperation<PaginatedUserGroupsRequestDto, PaginatedResult<Group>>
{
    public PaginatedUserGroupsOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<PaginatedResult<Group>> ExecuteAsync(AuditableRequestDto<PaginatedUserGroupsRequestDto> request)
    {
        var filter = request.Data;

        var groupIdsQuery = _groupContext.RepositoryContext.GroupMemberRepository.Query()
            .Where(m => m.UserId == filter.UserId)
            .Select(m => m.GroupId);

        var query = _groupContext.RepositoryContext.GroupRepository.Query()
            .Where(g => groupIdsQuery.Contains(g.Id));

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

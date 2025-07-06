using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class PageGroupMembersByGroupDto
{
    public Guid GroupId { get; set; } // REQUIRED
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public DateTime? JoinedAfter { get; set; }
    public DateTime? JoinedBefore { get; set; }
}

[OperationGroup("IAM Group Members")]
[OperationRoute("group/member/page-by-group")]
public class PageGroupMembersByGroupOperation
    : BaseGroupDomainOperation<PageGroupMembersByGroupDto, PaginatedResult<GroupMember>>
{
    public PageGroupMembersByGroupOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<PaginatedResult<GroupMember>> ExecuteAsync(
        AuditableRequestDto<PageGroupMembersByGroupDto> request)
    {
        var filter = request.Data;
        var query = _groupContext.RepositoryContext.GroupMemberRepository.Query()
            .Where(gm => gm.GroupId == filter.GroupId);

        if (filter.UserId.HasValue)
            query = query.Where(gm => gm.UserId == filter.UserId.Value);
        if (filter.RoleId.HasValue)
            query = query.Where(gm => gm.RoleId == filter.RoleId.Value);
        if (filter.JoinedAfter.HasValue)
            query = query.Where(gm => gm.JoinedAt >= filter.JoinedAfter.Value);
        if (filter.JoinedBefore.HasValue)
            query = query.Where(gm => gm.JoinedAt <= filter.JoinedBefore.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PaginatedResult<GroupMember>(items, totalCount, filter.Page, filter.PageSize);
    }
}

// --------- ListGroupMembersPagedOperation.cs ---------
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class ListGroupMembersPagedDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public Guid? GroupId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? JoinedAfter { get; set; }
    public DateTime? JoinedBefore { get; set; }
    // Add other filters as needed
}

[OperationGroup("Group Member")]
[OperationRoute("group-member/list")]
public class ListGroupMembersPagedOperation
    : BaseGroupMemberCrudOperation<ListGroupMembersPagedDto, PaginatedResult<GroupMember>>
{
    public ListGroupMembersPagedOperation(BaseIamEntityRepository<GroupMember> repository) : base(repository) { }

    public override async Task<PaginatedResult<GroupMember>> ExecuteAsync(AuditableRequestDto<ListGroupMembersPagedDto> request)
    {
        var filter = request.Data;
        var query = _repository.Query();

        if (filter.GroupId.HasValue)
            query = query.Where(gm => gm.GroupId == filter.GroupId.Value);
        if (filter.UserId.HasValue)
            query = query.Where(gm => gm.UserId == filter.UserId.Value);
        if (filter.RoleId.HasValue)
            query = query.Where(gm => gm.RoleId == filter.RoleId.Value);
        if (filter.IsActive.HasValue)
            query = query.Where(gm => gm.IsActive == filter.IsActive.Value);
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

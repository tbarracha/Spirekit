using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

public class PaginatedUserGroupsRequestDto
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public string? Name { get; set; }
    public Guid? OwnerUserId { get; set; }
    public Guid? GroupTypeId { get; set; }
}

[OperationGroup("IAM User Groups")]
[OperationRoute("user/groups/page")]
public class PaginatedUserGroupsOperation
    : BaseGroupCrudOperation<PaginatedUserGroupsRequestDto, PaginatedResult<Group>>
{
    private readonly BaseIamEntityRepository<GroupMember> _groupMemberRepository;

    public PaginatedUserGroupsOperation(
        BaseIamEntityRepository<Group> groupRepository,
        BaseIamEntityRepository<GroupMember> groupMemberRepository)
        : base(groupRepository)
    {
        _groupMemberRepository = groupMemberRepository;
    }

    public override async Task<PaginatedResult<Group>> ExecuteAsync(AuditableRequestDto<PaginatedUserGroupsRequestDto> request)
    {
        var filter = request.Data;

        var groupIdsQuery = _groupMemberRepository.Query()
            .Where(m => m.UserId == filter.UserId)
            .Select(m => m.GroupId);

        var query = _repository.Query()
            .Where(g => groupIdsQuery.Contains(g.Id));

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

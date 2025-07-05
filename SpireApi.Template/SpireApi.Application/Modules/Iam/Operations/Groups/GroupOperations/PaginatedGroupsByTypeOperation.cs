using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupOperations;

public class PaginatedGroupsByTypeRequestDto
{
    public Guid GroupTypeId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public string? Name { get; set; }
    public Guid? OwnerUserId { get; set; }
}

[OperationGroup("IAM Groups")]
[OperationRoute("groups/type/page")]
public class PaginatedGroupsByTypeOperation
    : BaseGroupCrudOperation<PaginatedGroupsByTypeRequestDto, PaginatedResult<Group>>
{
    public PaginatedGroupsByTypeOperation(BaseIamEntityRepository<Group> groupRepository)
        : base(groupRepository) { }

    public override async Task<PaginatedResult<Group>> ExecuteAsync(AuditableRequestDto<PaginatedGroupsByTypeRequestDto> request)
    {
        var filter = request.Data;

        var query = _repository.Query()
            .Where(g => g.GroupTypeId == filter.GroupTypeId);

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(g => g.Name.Contains(filter.Name));

        if (filter.OwnerUserId.HasValue)
            query = query.Where(g => g.OwnerUserId == filter.OwnerUserId.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PaginatedResult<Group>(items, totalCount, filter.Page, filter.PageSize);
    }
}

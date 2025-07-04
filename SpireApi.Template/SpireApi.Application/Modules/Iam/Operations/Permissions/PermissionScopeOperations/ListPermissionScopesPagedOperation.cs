// --------- ListPermissionScopesPagedOperation.cs ---------
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionScopeOperations;

public class ListPermissionScopesPagedDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Name { get; set; }
}

[OperationGroup("Permission Scope")]
[OperationRoute("permission-scope/list")]
public class ListPermissionScopesPagedOperation
    : BasePermissionScopeCrudOperation<ListPermissionScopesPagedDto, PaginatedResult<PermissionScope>>
{
    public ListPermissionScopesPagedOperation(BaseIamEntityRepository<PermissionScope> repository)
        : base(repository) { }

    public override async Task<PaginatedResult<PermissionScope>> ExecuteAsync(AuditableRequestDto<ListPermissionScopesPagedDto> request)
    {
        var filter = request.Data;
        var query = _repository.Query();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(ps => ps.Name.Contains(filter.Name));

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PaginatedResult<PermissionScope>(items, totalCount, filter.Page, filter.PageSize);
    }
}

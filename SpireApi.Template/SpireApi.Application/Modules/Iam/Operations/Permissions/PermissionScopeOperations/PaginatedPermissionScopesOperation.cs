// --------- ListPermissionScopesPagedOperation.cs ---------
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Permissions.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionScopeOperations;

public class PaginatedPermissionScopesRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Name { get; set; }
}

[OperationGroup("IAM Permission Scopes")]
[OperationRoute("permission/scopes/page")]
public class PaginatedPermissionScopesOperation
    : BasePermissionScopeCrudOperation<PaginatedPermissionScopesRequestDto, PaginatedResult<PermissionScope>>
{
    public PaginatedPermissionScopesOperation(BaseIamEntityRepository<PermissionScope> repository)
        : base(repository) { }

    public override async Task<PaginatedResult<PermissionScope>> ExecuteAsync(AuditableRequestDto<PaginatedPermissionScopesRequestDto> request)
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

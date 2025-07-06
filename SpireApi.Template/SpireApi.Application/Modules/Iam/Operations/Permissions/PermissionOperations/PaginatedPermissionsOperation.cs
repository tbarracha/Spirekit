// --------- ListPermissionsPagedOperation.cs ---------
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Permissions.PermissionOperations;

public class PaginatedPermissionsRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Name { get; set; }
    public Guid? PermissionScopeId { get; set; }
}

[OperationGroup("IAM Permissions")]
[OperationRoute("permission/page")]
public class PaginatedPermissionsOperation
    : BasePermissionCrudOperation<PaginatedPermissionsRequestDto, PaginatedResult<Permission>>
{
    public PaginatedPermissionsOperation(BaseIamEntityRepository<Permission> repository)
        : base(repository) { }

    public override async Task<PaginatedResult<Permission>> ExecuteAsync(AuditableRequestDto<PaginatedPermissionsRequestDto> request)
    {
        var filter = request.Data;
        var query = _repository.Query();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(p => p.Name.Contains(filter.Name));
        if (filter.PermissionScopeId.HasValue)
            query = query.Where(p => p.PermissionScopeId == filter.PermissionScopeId);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PaginatedResult<Permission>(items, totalCount, filter.Page, filter.PageSize);
    }
}

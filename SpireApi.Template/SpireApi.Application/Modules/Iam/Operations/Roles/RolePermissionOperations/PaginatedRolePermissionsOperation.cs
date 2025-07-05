// --------- ListRolePermissionsPagedOperation.cs ---------
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RolePermissionOperations;

public class PaginatedRolePermissionsRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? RoleId { get; set; }
    public Guid? PermissionId { get; set; }
}

[OperationGroup("IAM Role Permissions")]
[OperationRoute("role/permissions/page")]
public class PaginatedRolePermissionsOperation : BaseRolePermissionCrudOperation<PaginatedRolePermissionsRequestDto, PaginatedResult<RolePermission>>
{
    public PaginatedRolePermissionsOperation(BaseIamEntityRepository<RolePermission> repository) : base(repository) { }

    public override async Task<PaginatedResult<RolePermission>> ExecuteAsync(AuditableRequestDto<PaginatedRolePermissionsRequestDto> request)
    {
        var filter = request.Data;
        var query = _repository.Query();

        if (filter.RoleId.HasValue)
            query = query.Where(rp => rp.RoleId == filter.RoleId.Value);
        if (filter.PermissionId.HasValue)
            query = query.Where(rp => rp.PermissionId == filter.PermissionId.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PaginatedResult<RolePermission>(items, totalCount, filter.Page, filter.PageSize);
    }
}

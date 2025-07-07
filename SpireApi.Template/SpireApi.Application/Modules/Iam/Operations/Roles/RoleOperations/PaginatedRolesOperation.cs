// --------- PaginatedRolesOperation.cs ---------
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;

public class PaginatedRolesRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Name { get; set; }
    public Guid? AccountId { get; set; }
}

[OperationGroup("IAM Roles")]
[OperationRoute("roles/page")]
public class PaginatedRolesOperation : BaseRoleCrudOperation<PaginatedRolesRequestDto, PaginatedResult<Role>>
{
    public PaginatedRolesOperation(BaseIamEntityRepository<Role> repository) : base(repository) { }

    public override async Task<PaginatedResult<Role>> ExecuteAsync(AuditableRequestDto<PaginatedRolesRequestDto> request)
    {
        var filter = request.Data;
        var query = _repository.Query();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(r => r.Name.Contains(filter.Name));

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PaginatedResult<Role>(items, totalCount, filter.Page, filter.PageSize);
    }
}

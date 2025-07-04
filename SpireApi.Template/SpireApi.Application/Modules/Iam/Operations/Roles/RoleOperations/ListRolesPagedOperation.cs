// --------- ListRolesPagedOperation.cs ---------
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;

public class ListRolesPagedDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Name { get; set; }
    public Guid? AccountId { get; set; }
}

[OperationGroup("Role")]
[OperationRoute("role/list")]
public class ListRolesPagedOperation : BaseRoleCrudOperation<ListRolesPagedDto, PaginatedResult<Role>>
{
    public ListRolesPagedOperation(BaseIamEntityRepository<Role> repository) : base(repository) { }

    public override async Task<PaginatedResult<Role>> ExecuteAsync(AuditableRequestDto<ListRolesPagedDto> request)
    {
        var filter = request.data;
        var query = _repository.Query();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(r => r.Name.Contains(filter.Name));
        if (filter.AccountId.HasValue)
            query = query.Where(r => r.AccountId == filter.AccountId.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PaginatedResult<Role>(items, totalCount, filter.Page, filter.PageSize);
    }
}

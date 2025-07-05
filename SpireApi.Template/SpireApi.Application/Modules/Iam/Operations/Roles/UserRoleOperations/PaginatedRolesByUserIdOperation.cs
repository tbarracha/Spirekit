using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Application.Modules.Iam.Operations.Roles.RoleOperations;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;
using SpireCore.Lists.Pagination;

namespace SpireApi.Application.Modules.Iam.Operations.Roles.UserRoleOperations;

public class PaginatedRolesByUserIdRequestDto
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Name { get; set; }
}

[OperationGroup("IAM User Roles")]
[OperationRoute("user/roles/page")]
public class PaginatedRolesByUserIdOperation : BaseRoleCrudOperation<PaginatedRolesByUserIdRequestDto, PaginatedResult<Role>>
{
    private readonly BaseIamEntityRepository<UserRole> _userRoleRepository;

    public PaginatedRolesByUserIdOperation(
        BaseIamEntityRepository<Role> roleRepository,
        BaseIamEntityRepository<UserRole> userRoleRepository)
        : base(roleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public override async Task<PaginatedResult<Role>> ExecuteAsync(AuditableRequestDto<PaginatedRolesByUserIdRequestDto> request)
    {
        var filter = request.Data;

        var roleIdsQuery = _userRoleRepository.Query()
            .Where(ur => ur.UserId == filter.UserId)
            .Select(ur => ur.RoleId);

        var roleQuery = _repository.Query()
            .Where(role => roleIdsQuery.Contains(role.Id));

        if (!string.IsNullOrWhiteSpace(filter.Name))
            roleQuery = roleQuery.Where(role => role.Name.Contains(filter.Name));

        var totalCount = await roleQuery.CountAsync();
        var items = await roleQuery
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PaginatedResult<Role>(items, totalCount, filter.Page, filter.PageSize);
    }
}

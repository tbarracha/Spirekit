using Microsoft.Extensions.DependencyInjection;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;

namespace SpireApi.Application.Modules.Iam;

public static class IamServiceExtensions
{
    /// <summary>
    /// Register all IAM repositories for DI.
    /// </summary>
    public static IServiceCollection AddIamModuleServices(this IServiceCollection services)
    {
        // --- Groups ---
        //services.AddScoped<IRepository<Group, Guid>, GroupRepository>();
        services.AddScoped<BaseIamEntityRepository<Group>, GroupRepository>();
        services.AddScoped<BaseIamEntityRepository<GroupMember>, GroupMemberRepository>();
        services.AddScoped<BaseIamEntityRepository<GroupType>, GroupTypeRepository>();

        // --- Roles ---
        services.AddScoped<BaseIamEntityRepository<Role>, RoleRepository>();
        services.AddScoped<BaseIamEntityRepository<RolePermission>, RolePermissionRepository>();
        services.AddScoped<BaseIamEntityRepository<UserRole>, UserRoleRepository>();

        // --- Permissions ---
        services.AddScoped<BaseIamEntityRepository<Permission>, PermissionRepository>();
        services.AddScoped<BaseIamEntityRepository<PermissionScope>, PermissionScopeRepository>();

        return services;
    }
}

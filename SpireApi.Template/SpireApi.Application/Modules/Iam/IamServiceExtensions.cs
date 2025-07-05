using Microsoft.Extensions.DependencyInjection;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Users;
using SpireApi.Application.Modules.Iam.Domain.Models.Users.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Services;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam;

public static class IamServiceExtensions
{
    /// <summary>
    /// Register all IAM repositories for DI.
    /// </summary>
    public static IServiceCollection AddIamModuleServices(this IServiceCollection services)
    {
        // --- Users ---
        services.AddScoped<IamUserRepository>();
        services.AddScoped<BaseIamEntityRepository<IamUser>, IamUserRepository>();

        // --- Groups ---
        services.AddScoped<GroupRepository>();
        services.AddScoped<BaseIamEntityRepository<Group>, GroupRepository>();
        services.AddScoped<GroupMemberRepository>();
        services.AddScoped<BaseIamEntityRepository<GroupMember>, GroupMemberRepository>();
        services.AddScoped<GroupTypeRepository>();
        services.AddScoped<BaseIamEntityRepository<GroupType>, GroupTypeRepository>();

        // --- Roles ---
        services.AddScoped<RoleRepository>();
        services.AddScoped<BaseIamEntityRepository<Role>, RoleRepository>();
        services.AddScoped<RolePermissionRepository>();
        services.AddScoped<BaseIamEntityRepository<RolePermission>, RolePermissionRepository>();
        services.AddScoped<UserRoleRepository>();
        services.AddScoped<BaseIamEntityRepository<UserRole>, UserRoleRepository>();

        // --- Permissions ---
        services.AddScoped<PermissionRepository>();
        services.AddScoped<BaseIamEntityRepository<Permission>, PermissionRepository>();
        services.AddScoped<PermissionScopeRepository>();
        services.AddScoped<BaseIamEntityRepository<PermissionScope>, PermissionScopeRepository>();

        // --- Services ---
        services.AddScoped<IamService>();

        return services;
    }
}

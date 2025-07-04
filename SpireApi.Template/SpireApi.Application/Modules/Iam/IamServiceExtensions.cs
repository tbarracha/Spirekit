using Microsoft.Extensions.DependencyInjection;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;

namespace SpireApi.Application.Modules.Iam;

public static class IamServiceExtensions
{
    /// <summary>
    /// Register all IAM repositories for DI.
    /// </summary>
    public static IServiceCollection AddIamModuleServices(this IServiceCollection services)
    {
        // --- Groups ---
        services.AddScoped<IRepository<Group, Guid>, GroupRepository>();
        services.AddScoped<GroupMemberRepository>();
        services.AddScoped<GroupTypeRepository>();

        // --- Roles ---
        services.AddScoped<RoleRepository>();
        services.AddScoped<RolePermissionRepository>();

        // --- Permissions ---
        services.AddScoped<PermissionRepository>();
        services.AddScoped<PermissionScopeRepository>();

        return services;
    }
}

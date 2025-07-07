using Microsoft.Extensions.DependencyInjection;

namespace SpireApi.Application.Modules.Iam.Domain.Groups;

public static class GroupsModuleExtensions
{
    public static IServiceCollection AddGroupsModuleServices(this IServiceCollection services)
    {
        // register services here
        return services;
    }
}

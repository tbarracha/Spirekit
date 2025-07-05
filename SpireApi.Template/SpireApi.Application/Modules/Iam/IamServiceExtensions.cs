using Microsoft.Extensions.DependencyInjection;
using SpireApi.Application.Modules.Iam.Domain.Services;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.Services;

namespace SpireApi.Application.Modules.Iam;

public static class IamServiceExtensions
{
    public static IServiceCollection AddIamModuleServices(this IServiceCollection services)
    {
        // Register all BaseIamEntityRepository<> subclasses across all assemblies
        services.AddScopedImplementationsOfGenericType(typeof(BaseIamEntityRepository<>));

        // Add IAM service layer
        services.AddScoped<IamService>();

        return services;
    }
}

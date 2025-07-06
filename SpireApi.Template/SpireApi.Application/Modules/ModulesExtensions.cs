using Microsoft.Extensions.DependencyInjection;
using SpireApi.Application.Modules.Authentication;
using SpireApi.Application.Modules.Iam;
using SpireApi.Shared.Configuration.Modules;

namespace SpireApi.Application.Modules;

public static class ModulesExtensions
{
    public static void FilterEnabledModules(this IServiceCollection services, ModulesConfigurationList modules)
    {
        if (modules.TryGetValue("Auth", out var authConfig) && authConfig.Enabled)
        {
            services.AddAuthModuleServices();
            Console.WriteLine("[MODULE] Added Auth");
        }

        if (modules.TryGetValue("Iam", out var iamConfig) && iamConfig.Enabled)
        {
            services.AddIamModuleServices();
            Console.WriteLine("[MODULE] Added IAM");
        }
    }
}
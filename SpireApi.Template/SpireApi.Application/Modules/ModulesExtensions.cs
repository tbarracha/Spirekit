using Microsoft.Extensions.DependencyInjection;
using SpireApi.Application.Modules.Authentication;
using SpireCore.API.Configuration.Modules;

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
    }
}
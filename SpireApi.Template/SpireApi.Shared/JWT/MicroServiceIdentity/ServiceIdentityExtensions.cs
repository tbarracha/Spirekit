using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpireCore.Utils;

namespace SpireApi.Shared.JWT.MicroServiceIdentity;

public static class ServiceIdentityExtensions
{
    /// <summary>
    /// Registers the Service Identity and ServiceTokenProvider from appsettings.json.
    /// </summary>
    public static IServiceCollection AddServiceIdentityAndTokenProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceIdentitySection = configuration.GetSection("ServiceIdentity");
        var serviceIdentity = serviceIdentitySection.Get<ServiceIdentity>() ?? new ServiceIdentity();

        serviceIdentity.Id = GuidUtility.CreateDeterministicGuid(serviceIdentity.ServiceName);

        services.AddSingleton<IJwtServiceAuth>(serviceIdentity);
        services.AddSingleton<IServiceAuthenticationService, ServiceAuthenticationService>();
        services.AddSingleton<ServiceTokenProvider>();

        return services;
    }
}

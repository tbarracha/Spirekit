using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace {Namespace}.Shared.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddLifetimeServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(classes => classes.AssignableTo<ITransientService>())
                .AsSelf()
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<IScopedService>())
                .AsSelf()
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>())
                .AsSelf()
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );

        return services;
    }
}


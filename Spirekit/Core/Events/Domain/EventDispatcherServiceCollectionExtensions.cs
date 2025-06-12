using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Spirekit.Events.Domain.Extensions;

public static class EventDispatcherServiceCollectionExtensions
{
    public static IServiceCollection AddDomainEventDispatcher(this IServiceCollection services)
    {
        services.AddSingleton<IEventDispatcher, InMemoryEventDispatcher>();

        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}

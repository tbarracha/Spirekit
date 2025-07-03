using Microsoft.Extensions.DependencyInjection;

namespace SpireCore.Events.Dispatcher
{
    public static class EventDispatcherServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainEventDispatcher(this IServiceCollection services)
        {
            services.AddScoped<IEventDispatcher, InMemoryEventDispatcher>();

            // Get all types in all loaded assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var handlerInterfaceType = typeof(IEventHandler<>);

            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                    .Select(i => new { HandlerType = t, InterfaceType = i }))
                .ToList();

            foreach (var handler in handlerTypes)
            {
                services.AddTransient(handler.InterfaceType, handler.HandlerType);
            }

            return services;
        }
    }
}


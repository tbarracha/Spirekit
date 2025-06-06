
using Microsoft.Extensions.DependencyInjection;

namespace Spirekit.Events.Domain;

public class InMemoryEventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(@event, cancellationToken);
        }
    }
}

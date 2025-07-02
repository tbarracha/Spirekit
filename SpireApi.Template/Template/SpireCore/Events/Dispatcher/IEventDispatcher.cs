namespace SpireCore.Events.Dispatcher;

public interface IEventDispatcher
{
    Task DispatchEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}


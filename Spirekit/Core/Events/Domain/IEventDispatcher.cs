
namespace Spirekit.Events.Domain;

public interface IEventDispatcher
{
    Task DispatchEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}

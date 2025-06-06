
namespace Spirekit.Events.Domain;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}

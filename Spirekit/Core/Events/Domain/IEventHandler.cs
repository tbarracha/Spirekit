
namespace Spirekit.Events.Domain;

public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

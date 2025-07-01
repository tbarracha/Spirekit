namespace SpireCore.Events.Domain;

public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleEventAsync(TEvent @event, CancellationToken cancellationToken = default);
}

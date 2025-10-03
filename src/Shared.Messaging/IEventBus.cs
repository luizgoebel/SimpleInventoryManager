namespace Shared.Messaging;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event, CancellationToken ct = default);
    void Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;
}

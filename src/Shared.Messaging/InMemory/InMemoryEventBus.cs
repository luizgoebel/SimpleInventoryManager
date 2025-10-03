namespace Shared.Messaging.InMemory;

public class InMemoryEventBus : IEventBus
{
    private readonly IServiceProvider _sp;
    private readonly Dictionary<string, List<Type>> _handlers = new();

    public InMemoryEventBus(IServiceProvider sp) => _sp = sp;

    public Task PublishAsync(IntegrationEvent @event, CancellationToken ct = default)
    {
        if (_handlers.TryGetValue(@event.EventType, out var handlerTypes))
        {
            foreach (var handlerType in handlerTypes)
            {
                var handler = _sp.GetService(handlerType);
                if (handler is null) continue;
                var method = handlerType.GetMethod("HandleAsync");
                if (method != null)
                    _ = (Task?)method.Invoke(handler, new object[] { @event, ct });
            }
        }
        return Task.CompletedTask;
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>
    {
        var key = typeof(TEvent).Name;
        if (!_handlers.ContainsKey(key))
            _handlers[key] = new List<Type>();
        if (!_handlers[key].Contains(typeof(THandler)))
            _handlers[key].Add(typeof(THandler));
    }
}

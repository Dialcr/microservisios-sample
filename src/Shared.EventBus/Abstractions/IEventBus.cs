using Shared.EventBus.Events;

namespace Shared.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
    Task SubscribeAsync<T>(string queueName, string routingKey, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class;
}

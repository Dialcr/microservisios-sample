using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.EventBus.Abstractions;
using Shared.EventBus.Events;

namespace OrderService.Infrastructure.EventBus;

public class RabbitMqEventBus : IEventBus
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqEventBus> _logger;
    private readonly EventBusSettings _settings;

    public RabbitMqEventBus(IOptions<EventBusSettings> settings, ILogger<RabbitMqEventBus> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync("ecommerce", ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);

        var eventName = typeof(T).Name;
        var routingKey = eventName switch
        {
            nameof(OrderCreatedEvent) => "order.created",
            nameof(PaymentProcessedEvent) => "payment.processed",
            _ => eventName.ToLower()
        };

        var body = JsonSerializer.SerializeToUtf8Bytes(@event);
        var properties = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                { "retry-count", 0 }
            }
        };

        await channel.BasicPublishAsync(
            exchange: "ecommerce",
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Published event {EventName} with routing key {RoutingKey}", eventName, routingKey);
    }

    public Task SubscribeAsync<T>(string queueName, string routingKey, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException("Consumer should use dedicated background service");
    }
}

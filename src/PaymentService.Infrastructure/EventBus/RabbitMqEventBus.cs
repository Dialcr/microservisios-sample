using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.EventBus.Abstractions;

namespace PaymentService.Infrastructure.EventBus;

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

        var routingKey = typeof(T).Name switch
        {
            "OrderCreatedEvent" => "order.created",
            "PaymentProcessedEvent" => "payment.processed",
            _ => typeof(T).Name.ToLower()
        };

        var body = JsonSerializer.SerializeToUtf8Bytes(@event);
        await channel.BasicPublishAsync("ecommerce", routingKey, true, new BasicProperties { Persistent = true }, body, cancellationToken);
        _logger.LogInformation("Published {Event} with key {Key}", typeof(T).Name, routingKey);
    }

    public Task SubscribeAsync<T>(string queueName, string routingKey, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class
        => throw new NotImplementedException();
}

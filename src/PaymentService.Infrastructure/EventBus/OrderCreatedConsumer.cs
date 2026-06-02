using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.EventBus.Abstractions;
using Shared.EventBus.Events;

namespace PaymentService.Infrastructure.EventBus;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly EventBusSettings _settings;

    public OrderCreatedConsumer(IServiceScopeFactory scopeFactory, IOptions<EventBusSettings> settings, ILogger<OrderCreatedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password
        };

        using var connection = await factory.CreateConnectionAsync(stoppingToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync("ecommerce", ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);

        var queueName = "payment-order-created-queue";
        var dlqName = "payment-order-created-dlq";

        var dlqArgs = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", "ecommerce" },
            { "x-dead-letter-routing-key", "order.created.dlq" }
        };

        await channel.QueueDeclareAsync(dlqName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, arguments: dlqArgs, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(queueName, "ecommerce", "order.created", cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                if (orderEvent is not null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await mediator.Send(new PaymentService.Application.Commands.ProcessPayment.ProcessPaymentCommand(
                        orderEvent.OrderId, orderEvent.TotalAmount), stoppingToken);
                }

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OrderCreated event");
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        _logger.LogInformation("OrderCreatedConsumer started");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

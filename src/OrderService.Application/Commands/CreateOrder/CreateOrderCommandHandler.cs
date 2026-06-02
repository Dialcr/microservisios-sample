using MediatR;
using OrderService.Domain.Entities;
using OrderService.Application.Interfaces;
using Shared.EventBus.Abstractions;
using Shared.EventBus.Events;

namespace OrderService.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductGrpcClient _productGrpcClient;
    private readonly IEventBus _eventBus;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductGrpcClient productGrpcClient,
        IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _productGrpcClient = productGrpcClient;
        _eventBus = eventBus;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        foreach (var item in request.Items)
        {
            var product = await _productGrpcClient.GetProductAsync(item.ProductId);
            if (product is null)
                throw new DllNotFoundException($"Product {item.ProductId} not found");
        }

        var orderItems = request.Items
            .Select(i => new OrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity))
            .ToList();

        var order = new Order(request.UserId, orderItems);
        await _orderRepository.AddAsync(order, cancellationToken);

        await _eventBus.PublishAsync(new OrderCreatedEvent(
            order.Id, order.UserId, order.TotalAmount, order.CreatedAt), cancellationToken);

        return order.Id;
    }
}

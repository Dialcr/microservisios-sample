using MediatR;

namespace OrderService.Application.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid UserId,
    List<OrderItemDto> Items) : IRequest<Guid>;

public record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);

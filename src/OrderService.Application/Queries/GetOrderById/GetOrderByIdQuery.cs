using MediatR;
using OrderService.Domain.Entities;

namespace OrderService.Application.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<Order?>;

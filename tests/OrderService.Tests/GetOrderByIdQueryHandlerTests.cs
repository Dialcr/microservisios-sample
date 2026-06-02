using FluentAssertions;
using Moq;
using OrderService.Application.Queries.GetOrderById;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Tests;

public class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingOrder_ShouldReturnOrder()
    {
        var mockRepo = new Mock<IOrderRepository>();
        var order = new Order(Guid.NewGuid(), new List<OrderItem>());
        mockRepo.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var handler = new GetOrderByIdQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Status.Should().Be("Pending");
    }
}

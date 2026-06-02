using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Interfaces;
using OrderService.Application.Dtos;
using OrderService.Domain.Entities;
using Shared.EventBus.Abstractions;
using Shared.EventBus.Events;

namespace OrderService.Tests;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IProductGrpcClient> _grpcMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _grpcMock = new Mock<IProductGrpcClient>();
        _eventBusMock = new Mock<IEventBus>();
        _handler = new CreateOrderCommandHandler(_orderRepoMock.Object, _grpcMock.Object, _eventBusMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateOrderAndPublishEvent()
    {
        var productId = Guid.NewGuid();
        var command = new CreateOrderCommand(Guid.NewGuid(), new List<OrderItemDto>
        {
            new(productId, "Laptop", 1500, 1)
        });

        _grpcMock.Setup(g => g.GetProductAsync(productId)).ReturnsAsync(new ProductDto
        {
            Id = productId, Name = "Laptop", Price = 1500, StockQuantity = 10
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _eventBusMock.Verify(e => e.PublishAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ShouldThrowException()
    {
        var productId = Guid.NewGuid();
        var command = new CreateOrderCommand(Guid.NewGuid(), new List<OrderItemDto>
        {
            new(productId, "Unknown", 100, 1)
        });

        _grpcMock.Setup(g => g.GetProductAsync(productId)).ReturnsAsync((ProductDto?)null);

        await FluentActions.Awaiting(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }
}

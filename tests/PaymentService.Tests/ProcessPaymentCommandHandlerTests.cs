using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Commands.ProcessPayment;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using Shared.EventBus.Abstractions;
using Shared.EventBus.Events;

namespace PaymentService.Tests;

public class ProcessPaymentCommandHandlerTests
{
    private readonly Mock<IPaymentRepository> _repoMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly ProcessPaymentCommandHandler _handler;

    public ProcessPaymentCommandHandlerTests()
    {
        _repoMock = new Mock<IPaymentRepository>();
        _eventBusMock = new Mock<IEventBus>();
        var loggerMock = new Mock<ILogger<ProcessPaymentCommandHandler>>();
        _handler = new ProcessPaymentCommandHandler(_repoMock.Object, _eventBusMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidPayment_ShouldProcessAndPublishEvent()
    {
        var command = new ProcessPaymentCommand(Guid.NewGuid(), 1500);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        _repoMock.Verify(r => r.AddAsync(It.Is<Payment>(p =>
            p.Amount == 1500 && p.Status == "Completed"), It.IsAny<CancellationToken>()), Times.Once);
        _eventBusMock.Verify(e => e.PublishAsync(It.IsAny<PaymentProcessedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Commands.SendNotification;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Tests;

public class SendNotificationCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidNotification_ShouldSaveAndLog()
    {
        var mockRepo = new Mock<INotificationRepository>();
        var loggerMock = new Mock<ILogger<SendNotificationCommandHandler>>();
        var handler = new SendNotificationCommandHandler(mockRepo.Object, loggerMock.Object);

        var command = new SendNotificationCommand(Guid.NewGuid(), "Payment received", "customer@test.com");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        mockRepo.Verify(r => r.AddAsync(It.Is<Notification>(n =>
            n.Message == "Payment received"), It.IsAny<CancellationToken>()), Times.Once);
    }
}

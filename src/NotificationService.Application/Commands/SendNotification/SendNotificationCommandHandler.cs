using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Entities;
using NotificationService.Application.Interfaces;

namespace NotificationService.Application.Commands.SendNotification;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Guid>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<SendNotificationCommandHandler> _logger;

    public SendNotificationCommandHandler(
        INotificationRepository notificationRepository,
        ILogger<SendNotificationCommandHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new Notification(request.OrderId, request.Message, request.RecipientEmail);

        await _notificationRepository.AddAsync(notification, cancellationToken);

        _logger.LogInformation("Notification sent to {Recipient} for order {OrderId}",
            request.RecipientEmail, request.OrderId);

        return notification.Id;
    }
}

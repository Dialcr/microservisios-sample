using MediatR;

namespace NotificationService.Application.Commands.SendNotification;

public record SendNotificationCommand(Guid OrderId, string Message, string RecipientEmail) : IRequest<Guid>;

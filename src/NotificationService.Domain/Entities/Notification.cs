using NotificationService.Domain.Primitives;

namespace NotificationService.Domain.Entities;

public class Notification : Entity
{
    public Guid OrderId { get; private set; }
    public string Message { get; private set; }
    public string RecipientEmail { get; private set; }
    public DateTime SentAt { get; private set; }

    private Notification() { Message = null!; RecipientEmail = null!; }

    public Notification(Guid orderId, string message, string recipientEmail) : base()
    {
        OrderId = orderId;
        Message = message;
        RecipientEmail = recipientEmail;
        SentAt = DateTime.UtcNow;
    }
}

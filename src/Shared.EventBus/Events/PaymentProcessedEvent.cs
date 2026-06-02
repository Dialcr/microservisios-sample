namespace Shared.EventBus.Events;

public record PaymentProcessedEvent(
    Guid OrderId,
    Guid PaymentId,
    string Status,
    DateTime ProcessedAt)
{
    public PaymentProcessedEvent() : this(Guid.Empty, Guid.Empty, string.Empty, DateTime.UtcNow) { }
}

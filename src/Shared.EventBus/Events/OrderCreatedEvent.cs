namespace Shared.EventBus.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid UserId,
    decimal TotalAmount,
    DateTime CreatedAt)
{
    public OrderCreatedEvent() : this(Guid.Empty, Guid.Empty, 0, DateTime.UtcNow) { }
}

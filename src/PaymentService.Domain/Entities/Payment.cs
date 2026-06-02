using PaymentService.Domain.Primitives;

namespace PaymentService.Domain.Entities;

public class Payment : Entity
{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Payment() { Status = null!; }

    public Payment(Guid orderId, decimal amount) : base()
    {
        OrderId = orderId;
        Amount = amount;
        Status = "Pending";
        CreatedAt = DateTime.UtcNow;
    }

    public void Process()
    {
        Status = "Completed";
        ProcessedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = "Failed";
        ProcessedAt = DateTime.UtcNow;
    }
}

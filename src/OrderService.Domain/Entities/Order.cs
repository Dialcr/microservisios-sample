using OrderService.Domain.Primitives;

namespace OrderService.Domain.Entities;

public class Order : Entity
{
    public Guid UserId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<OrderItem> Items { get; private set; }

    private Order() { Status = null!; Items = null!; }

    public Order(Guid userId, List<OrderItem> items) : base()
    {
        UserId = userId;
        Items = items;
        TotalAmount = items.Sum(i => i.UnitPrice * i.Quantity);
        Status = "Pending";
        CreatedAt = DateTime.UtcNow;
    }
}

using ProductService.Domain.Primitives;

namespace ProductService.Domain.Entities;

public class Product : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Product() { Name = null!; Description = null!; }

    public Product(string name, string description, decimal price, int stockQuantity) : base()
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
    }
}

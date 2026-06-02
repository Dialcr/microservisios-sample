using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence;

public static class OrderDbContextSeed
{
    public static readonly Guid ProductLaptopId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid ProductMouseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid ProductKeyboardId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public static async Task SeedAsync(OrderDbContext context)
    {
        if (await context.Orders.AnyAsync()) return;

        var order1 = new Order(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            new List<OrderItem>
            {
                new(ProductLaptopId, "Laptop", 1299.99m, 1),
                new(ProductMouseId, "Mouse", 49.99m, 2)
            });

        var order2 = new Order(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            new List<OrderItem>
            {
                new(ProductKeyboardId, "Keyboard", 89.99m, 1)
            });

        context.Orders.AddRange(order1, order2);
        await context.SaveChangesAsync();
    }
}

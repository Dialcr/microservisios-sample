using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Persistence;

public static class ProductDbContextSeed
{
    public static readonly Guid LaptopId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid MouseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid KeyboardId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    public static readonly Guid MonitorId = Guid.Parse("66666666-6666-6666-6666-666666666666");

    public static async Task SeedAsync(ProductDbContext context)
    {
        if (await context.Products.AnyAsync()) return;

        var laptop = new Product("Laptop", "High-performance laptop with 32GB RAM", 1299.99m, 15);
        laptop.SetId(LaptopId);
        var mouse = new Product("Mouse", "Wireless ergonomic mouse", 49.99m, 50);
        mouse.SetId(MouseId);
        var keyboard = new Product("Keyboard", "Mechanical keyboard RGB", 89.99m, 30);
        keyboard.SetId(KeyboardId);
        var monitor = new Product("Monitor", "4K UHD 27-inch monitor", 399.99m, 20);
        monitor.SetId(MonitorId);

        context.Products.AddRange(laptop, mouse, keyboard, monitor);
        await context.SaveChangesAsync();
    }
}

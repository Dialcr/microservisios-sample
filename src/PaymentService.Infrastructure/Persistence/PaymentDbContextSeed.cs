using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Persistence;

public static class PaymentDbContextSeed
{
    public static async Task SeedAsync(PaymentDbContext context)
    {
        if (await context.Payments.AnyAsync()) return;

        var payment1 = new Payment(Guid.NewGuid(), 1399.97m);
        payment1.Process();

        var payment2 = new Payment(Guid.NewGuid(), 89.99m);
        payment2.Process();

        context.Payments.AddRange(payment1, payment2);
        await context.SaveChangesAsync();
    }
}

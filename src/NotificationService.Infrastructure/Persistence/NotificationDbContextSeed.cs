using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence;

public static class NotificationDbContextSeed
{
    public static async Task SeedAsync(NotificationDbContext context)
    {
        if (await context.Notifications.AnyAsync()) return;

        var notif1 = new Notification(Guid.NewGuid(), "Payment Completed for order", "john@example.com");
        var notif2 = new Notification(Guid.NewGuid(), "Payment Completed for order", "jane@example.com");

        context.Notifications.AddRange(notif1, notif2);
        await context.SaveChangesAsync();
    }
}

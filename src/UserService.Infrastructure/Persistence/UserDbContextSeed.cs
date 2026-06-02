using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;

public static class UserDbContextSeed
{
    public static readonly Guid JohnId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid JaneId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static async Task SeedAsync(UserDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var john = new User("John Doe", "john@example.com");
        john.SetId(JohnId);
        var jane = new User("Jane Smith", "jane@example.com");
        jane.SetId(JaneId);

        context.Users.AddRange(john, jane);
        await context.SaveChangesAsync();
    }
}

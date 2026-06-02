using UserService.Domain.Primitives;

namespace UserService.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { Name = null!; Email = null!; }

    public User(string name, string email) : base()
    {
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }
}

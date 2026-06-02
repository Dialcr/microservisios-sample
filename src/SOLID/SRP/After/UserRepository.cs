namespace SOLID.SRP.After;

public class UserRepository
{
    public void Save(string name, string email)
    {
        Console.WriteLine($"Saved user {name} to database");
    }
}

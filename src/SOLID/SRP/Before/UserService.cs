namespace SOLID.SRP.Before;

public class UserService
{
    public void Register(string name, string email)
    {
        Validate(name, email);
        SaveToDatabase(name, email);
        SendEmail(email);
    }

    private void Validate(string name, string email)
    {
        if (string.IsNullOrEmpty(name)) throw new Exception("Name required");
        if (string.IsNullOrEmpty(email)) throw new Exception("Email required");
    }

    private void SaveToDatabase(string name, string email)
    {
        Console.WriteLine($"Saved user {name} to database");
    }

    private void SendEmail(string email)
    {
        Console.WriteLine($"Sent welcome email to {email}");
    }
}

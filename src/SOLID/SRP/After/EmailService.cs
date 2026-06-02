namespace SOLID.SRP.After;

public class EmailService
{
    public void SendWelcomeEmail(string email)
    {
        Console.WriteLine($"Sent welcome email to {email}");
    }
}

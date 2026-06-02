namespace SOLID.DIP.Before;

public class EmailService
{
    public void Send(string message)
    {
        Console.WriteLine($"Sending email: {message}");
    }
}

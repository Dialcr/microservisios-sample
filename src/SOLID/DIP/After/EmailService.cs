namespace SOLID.DIP.After;

public class EmailService : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine($"Sending email: {message}");
    }
}

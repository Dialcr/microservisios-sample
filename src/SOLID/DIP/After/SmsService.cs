namespace SOLID.DIP.After;

public class SmsService : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine($"Sending SMS: {message}");
    }
}

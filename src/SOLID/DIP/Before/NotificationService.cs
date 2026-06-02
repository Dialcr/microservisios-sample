namespace SOLID.DIP.Before;

public class NotificationService
{
    private readonly EmailService _emailService;

    public NotificationService()
    {
        _emailService = new EmailService();
    }

    public void Notify(string message)
    {
        _emailService.Send(message);
    }
}

namespace SOLID.SRP.After;

public class UserService
{
    private readonly UserValidator _validator;
    private readonly UserRepository _repository;
    private readonly EmailService _emailService;

    public UserService(UserValidator validator, UserRepository repository, EmailService emailService)
    {
        _validator = validator;
        _repository = repository;
        _emailService = emailService;
    }

    public void Register(string name, string email)
    {
        _validator.Validate(name, email);
        _repository.Save(name, email);
        _emailService.SendWelcomeEmail(email);
    }
}

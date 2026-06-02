namespace SOLID.SRP.After;

public class UserValidator
{
    public void Validate(string name, string email)
    {
        if (string.IsNullOrEmpty(name)) throw new Exception("Name required");
        if (string.IsNullOrEmpty(email)) throw new Exception("Email required");
    }
}

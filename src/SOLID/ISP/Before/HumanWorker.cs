namespace SOLID.ISP.Before;

public class HumanWorker : IWorker
{
    public void Work() => Console.WriteLine("Working");
    public void Eat() => Console.WriteLine("Eating");
    public void Sleep() => Console.WriteLine("Sleeping");
}

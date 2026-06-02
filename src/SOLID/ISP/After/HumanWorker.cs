namespace SOLID.ISP.After;

public class HumanWorker : IWorkable, IEatable, ISleepable
{
    public void Work() => Console.WriteLine("Working");
    public void Eat() => Console.WriteLine("Eating");
    public void Sleep() => Console.WriteLine("Sleeping");
}

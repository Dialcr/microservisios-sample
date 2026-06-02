namespace SOLID.ISP.Before;

public class RobotWorker : IWorker
{
    public void Work() => Console.WriteLine("Working");
    public void Eat() => throw new NotImplementedException("Robot does not eat");
    public void Sleep() => throw new NotImplementedException("Robot does not sleep");
}

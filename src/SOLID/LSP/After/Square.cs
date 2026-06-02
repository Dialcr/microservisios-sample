namespace SOLID.LSP.After;

public class Square : IShape
{
    public int Side { get; set; }

    public int GetArea() => Side * Side;
}

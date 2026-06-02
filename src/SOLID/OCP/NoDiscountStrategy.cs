namespace SOLID.OCP;

public class NoDiscountStrategy : IDiscountStrategy
{
    public decimal CalculateDiscount(decimal price) => 0;
}

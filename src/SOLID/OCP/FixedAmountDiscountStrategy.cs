namespace SOLID.OCP;

public class FixedAmountDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _amount;

    public FixedAmountDiscountStrategy(decimal amount)
    {
        _amount = amount;
    }

    public decimal CalculateDiscount(decimal price) => Math.Min(_amount, price);
}

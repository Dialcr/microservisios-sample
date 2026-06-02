namespace SOLID.OCP;

public class PercentageDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _percentage;

    public PercentageDiscountStrategy(decimal percentage)
    {
        _percentage = percentage;
    }

    public decimal CalculateDiscount(decimal price) => price * _percentage / 100;
}

namespace SOLID.OCP;

public class SeasonalDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _baseDiscount;
    private readonly decimal _bonusDiscount;

    public SeasonalDiscountStrategy(decimal baseDiscount, decimal bonusDiscount)
    {
        _baseDiscount = baseDiscount;
        _bonusDiscount = bonusDiscount;
    }

    public decimal CalculateDiscount(decimal price) => price * (_baseDiscount + _bonusDiscount) / 100;
}

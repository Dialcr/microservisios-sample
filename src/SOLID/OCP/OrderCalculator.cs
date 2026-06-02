namespace SOLID.OCP;

public class OrderCalculator
{
    private readonly IDiscountStrategy _discountStrategy;

    public OrderCalculator(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }

    public decimal CalculateTotal(decimal price)
    {
        var discount = _discountStrategy.CalculateDiscount(price);
        return price - discount;
    }
}

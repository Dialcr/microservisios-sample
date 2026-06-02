namespace SOLID.OCP;

public interface IDiscountStrategy
{
    decimal CalculateDiscount(decimal price);
}

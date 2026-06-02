using OrderService.Application.Dtos;

namespace OrderService.Application.Interfaces;

public interface IProductGrpcClient
{
    Task<ProductDto?> GetProductAsync(Guid productId);
}

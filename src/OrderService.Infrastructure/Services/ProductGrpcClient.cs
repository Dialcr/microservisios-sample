using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Protos;

namespace OrderService.Infrastructure.Services;

public class ProductGrpcClient : IProductGrpcClient
{
    private readonly Protos.ProductGrpc.ProductGrpcClient _client;
    private readonly ILogger<ProductGrpcClient> _logger;

    public ProductGrpcClient(IConfiguration configuration, ILogger<ProductGrpcClient> logger)
    {
        _logger = logger;
        var grpcUrl = configuration.GetValue<string>("GrpcSettings:ProductServiceUrl") ?? "http://localhost:5002";
        var channel = GrpcChannel.ForAddress(grpcUrl);
        _client = new Protos.ProductGrpc.ProductGrpcClient(channel);
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        try
        {
            var request = new ProductRequest { Id = productId.ToString() };
            var response = await _client.GetProductAsync(request);

            return new ProductDto
            {
                Id = Guid.Parse(response.Id),
                Name = response.Name,
                Description = response.Description,
                Price = (decimal)response.Price,
                StockQuantity = response.StockQuantity
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product {ProductId} via gRPC", productId);
            return null;
        }
    }
}

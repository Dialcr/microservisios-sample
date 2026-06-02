using Grpc.Core;
using MediatR;
using ProductService.Api.Protos;
using ProductService.Application.Queries.GetProductById;

namespace ProductService.Api.Services;

public class ProductGrpcService : Protos.ProductGrpc.ProductGrpcBase
{
    private readonly IMediator _mediator;

    public ProductGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<ProductResponse> GetProduct(ProductRequest request, ServerCallContext context)
    {
        var id = Guid.Parse(request.Id);
        var product = await _mediator.Send(new GetProductByIdQuery(id));

        if (product is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));

        return new ProductResponse
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Description = product.Description,
            Price = (double)product.Price,
            StockQuantity = product.StockQuantity
        };
    }
}

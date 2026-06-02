using MediatR;
using ProductService.Domain.Entities;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(request.Name, request.Description, request.Price, request.StockQuantity);
        await _productRepository.AddAsync(product, cancellationToken);
        return product.Id;
    }
}

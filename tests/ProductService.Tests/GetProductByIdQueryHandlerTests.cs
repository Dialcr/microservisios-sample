using FluentAssertions;
using Moq;
using ProductService.Application.Interfaces;
using ProductService.Application.Queries.GetProductById;
using ProductService.Domain.Entities;

namespace ProductService.Tests;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingProduct_ShouldReturnProduct()
    {
        var mockRepo = new Mock<IProductRepository>();
        var product = new Product("Mouse", "Wireless", 50, 20);
        mockRepo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var handler = new GetProductByIdQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Mouse");
    }
}

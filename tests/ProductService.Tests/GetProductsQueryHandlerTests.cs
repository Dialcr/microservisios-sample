using FluentAssertions;
using Moq;
using ProductService.Application.Interfaces;
using ProductService.Application.Queries.GetProducts;
using ProductService.Domain.Entities;

namespace ProductService.Tests;

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllProducts()
    {
        var mockRepo = new Mock<IProductRepository>();
        var products = new List<Product>
        {
            new("A", "Desc A", 10, 5),
            new("B", "Desc B", 20, 3)
        };
        mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);

        var handler = new GetProductsQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }
}

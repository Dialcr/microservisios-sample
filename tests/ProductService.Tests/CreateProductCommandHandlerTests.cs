using FluentAssertions;
using Moq;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Tests;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _handler = new CreateProductCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateProduct()
    {
        var command = new CreateProductCommand("Laptop", "Gaming laptop", 1500, 10);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Product>(p =>
            p.Name == "Laptop" && p.Price == 1500), It.IsAny<CancellationToken>()), Times.Once);
    }
}

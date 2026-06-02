using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests;

public class ProductServiceIntegrationTests : IClassFixture<ProductServiceFactory>
{
    private readonly WebApplicationFactory<ProductService.Api.ProductServiceApiMarker> _factory;

    public ProductServiceIntegrationTests(ProductServiceFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateProduct_ValidRequest_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        var request = new { Name = "Test Product", Description = "A test", Price = 99.99, StockQuantity = 10 };

        var response = await client.PostAsJsonAsync("/api/products", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/products");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}

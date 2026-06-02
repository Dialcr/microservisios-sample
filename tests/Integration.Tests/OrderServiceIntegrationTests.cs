using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests;

public class OrderServiceIntegrationTests : IClassFixture<OrderServiceFactory>
{
    private readonly WebApplicationFactory<OrderService.Api.OrderServiceApiMarker> _factory;

    public OrderServiceIntegrationTests(OrderServiceFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateOrder_WithValidItems_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        var request = new
        {
            UserId = Guid.NewGuid(),
            Items = new[]
            {
                new { ProductId = Guid.NewGuid(), ProductName = "Widget", UnitPrice = 19.99m, Quantity = 2 }
            }
        };

        var response = await client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetOrderById_NonExisting_ReturnsNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/orders/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}

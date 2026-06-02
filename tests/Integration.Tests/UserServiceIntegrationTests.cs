using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests;

public class UserServiceIntegrationTests : IClassFixture<UserServiceFactory>
{
    private readonly WebApplicationFactory<UserService.Api.UserServiceApiMarker> _factory;

    public UserServiceIntegrationTests(UserServiceFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateUser_ValidRequest_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        var request = new { Name = "Test User", Email = "test@example.com" };

        var response = await client.PostAsJsonAsync("/api/users", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUser_ExistingUser_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var request = new { Name = "Get User", Email = "get@example.com" };
        var createResponse = await client.PostAsJsonAsync("/api/users", request);
        var userId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await client.GetAsync($"/api/users/{userId}");

        getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUser_NonExistingUser_ReturnsNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/users/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}

using FluentAssertions;
using Moq;
using UserService.Application.Interfaces;
using UserService.Application.Queries.GetUserById;
using UserService.Domain.Entities;

namespace UserService.Tests;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new GetUserByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ShouldReturnUser()
    {
        var userId = Guid.NewGuid();
        var user = new User("Jane", "jane@test.com");
        _repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Jane");
    }

    [Fact]
    public async Task Handle_NonExistingUser_ShouldReturnNull()
    {
        var userId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

        result.Should().BeNull();
    }
}

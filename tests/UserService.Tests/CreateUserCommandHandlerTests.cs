using FluentAssertions;
using Moq;
using UserService.Application.Commands.CreateUser;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Tests;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new CreateUserCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateUserAndReturnId()
    {
        var command = new CreateUserCommand("John Doe", "john@example.com");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Name == "John Doe" && u.Email == "john@example.com"), It.IsAny<CancellationToken>()), Times.Once);
    }
}

using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<User?>;

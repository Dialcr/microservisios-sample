using MediatR;
using ProductService.Domain.Entities;

namespace ProductService.Application.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<Product?>;

using MediatR;
using ProductService.Domain.Entities;

namespace ProductService.Application.Queries.GetProducts;

public record GetProductsQuery : IRequest<IEnumerable<Product>>;

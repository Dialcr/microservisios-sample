using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using ProductService.Application.Interfaces;
using Shared.EventBus.Abstractions;
using UserService.Application.Interfaces;

namespace Integration.Tests;

public class UserServiceFactory : WebApplicationFactory<UserService.Api.UserServiceApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserRepository));
            if (descriptor is not null) services.Remove(descriptor);

            var users = new List<UserService.Domain.Entities.User>();
            var mock = new Mock<IUserRepository>();

            mock.Setup(r => r.AddAsync(It.IsAny<UserService.Domain.Entities.User>(), It.IsAny<CancellationToken>()))
                .Callback<UserService.Domain.Entities.User, CancellationToken>((u, _) => users.Add(u))
                .Returns(Task.CompletedTask);

            mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) => users.SingleOrDefault(u => u.Id == id));

            services.AddScoped(_ => mock.Object);
        });
    }
}

public class ProductServiceFactory : WebApplicationFactory<ProductService.Api.ProductServiceApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IProductRepository));
            if (descriptor is not null) services.Remove(descriptor);

            var products = new List<ProductService.Domain.Entities.Product>();
            var mock = new Mock<IProductRepository>();

            mock.Setup(r => r.AddAsync(It.IsAny<ProductService.Domain.Entities.Product>(), It.IsAny<CancellationToken>()))
                .Callback<ProductService.Domain.Entities.Product, CancellationToken>((p, _) => products.Add(p))
                .Returns(Task.CompletedTask);

            mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) => products.SingleOrDefault(p => p.Id == id));

            mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((CancellationToken _) => products.ToList());

            services.AddScoped(_ => mock.Object);
        });
    }
}

public class OrderServiceFactory : WebApplicationFactory<OrderService.Api.OrderServiceApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var repoDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IOrderRepository));
            if (repoDescriptor is not null) services.Remove(repoDescriptor);

            var orders = new List<Order>();
            var mockRepo = new Mock<IOrderRepository>();

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Callback<Order, CancellationToken>((o, _) => orders.Add(o))
                .Returns(Task.CompletedTask);

            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) => orders.SingleOrDefault(o => o.Id == id));

            services.AddScoped(_ => mockRepo.Object);

            var grpcDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IProductGrpcClient));
            if (grpcDescriptor is not null) services.Remove(grpcDescriptor);

            var mockGrpc = new Mock<IProductGrpcClient>();
            mockGrpc.Setup(x => x.GetProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ProductDto { Id = Guid.NewGuid(), Name = "Test Product", Price = 10.0m, StockQuantity = 100 });

            services.AddScoped(_ => mockGrpc.Object);

            var eventBusDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEventBus));
            if (eventBusDescriptor is not null) services.Remove(eventBusDescriptor);

            var mockEventBus = new Mock<IEventBus>();
            services.AddScoped(_ => mockEventBus.Object);
        });
    }
}

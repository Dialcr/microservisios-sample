# MicroservicesDemo

A comprehensive .NET 10 microservices educational project demonstrating Clean Architecture, CQRS, RabbitMQ, gRPC, REST APIs, SOLID principles, and testing patterns.

## Architecture

```
User Service (REST) ────┐
Product Service (REST) ──┤
Order Service (REST) ────┤─── RabbitMQ ──→ Payment Service ──→ RabbitMQ ──→ Notification Service
                         │
                    gRPC (sync)
                         │
                    Product Service
```

## Microservices

| Service | Port | Database | Description |
|---------|------|----------|-------------|
| User Service | 5001 | PostgreSQL (5432) | User registration and querying |
| Product Service | 5002 | PostgreSQL (5433) | Product management and inventory |
| Order Service | 5003 | PostgreSQL (5434) | Order creation with gRPC product validation |
| Payment Service | 5004 | PostgreSQL (5435) | Async payment processing via RabbitMQ |
| Notification Service | 5005 | PostgreSQL (5436) | Email notification simulation |

## Technologies

- **.NET 10** - Latest .NET version
- **Clean Architecture** - Domain, Application, Infrastructure, API layers
- **CQRS with MediatR** - Command/Query separation
- **Entity Framework Core** - ORM with PostgreSQL
- **RabbitMQ** - Async event-driven communication
- **gRPC** - Sync inter-service communication (Order → Product)
- **REST APIs** - External facing APIs with Swagger/OpenAPI
- **Serilog** - Structured logging
- **Docker** - Containerized deployment
- **xUnit + FluentAssertions + Moq** - Testing

## SOLID Principles

See `src/SOLID/` for comprehensive examples of each principle:
- SRP: Single Responsibility Principle (Before/After)
- OCP: Open/Closed Principle (Discount Strategy pattern)
- LSP: Liskov Substitution Principle (Before/After with Shape hierarchy)
- ISP: Interface Segregation Principle (Before/After with worker interfaces)
- DIP: Dependency Inversion Principle (Before/After with message senders)

## How to Run

### Prerequisites

- .NET 10 SDK
- Docker and Docker Compose
- PostgreSQL (optional, for local development)

### Running with Docker (recommended)

```bash
docker compose up -d --build
```

This will start:
- 5 PostgreSQL instances (one per service)
- RabbitMQ with Management UI (port 15672)
- All 5 microservices

### Running locally

1. Start PostgreSQL and RabbitMQ:
```bash
docker compose up -d postgres-users postgres-products postgres-orders postgres-payments postgres-notifications rabbitmq
```

2. Run each service individually:
```bash
cd src/UserService.Api && dotnet run --urls http://localhost:5001
cd src/ProductService.Api && dotnet run --urls http://localhost:5002
cd src/OrderService.Api && dotnet run --urls http://localhost:5003
cd src/PaymentService.Api && dotnet run --urls http://localhost:5004
cd src/NotificationService.Api && dotnet run --urls http://localhost:5005
```

### Running Tests

```bash
# All tests
dotnet test

# Unit tests only
dotnet test tests/UserService.Tests
dotnet test tests/ProductService.Tests
dotnet test tests/OrderService.Tests
dotnet test tests/PaymentService.Tests
dotnet test tests/NotificationService.Tests

# Integration tests
dotnet test tests/Integration.Tests
```

## API Endpoints

### User Service (http://localhost:5001/swagger)
- `POST /api/users` - Create user
- `GET /api/users/{id}` - Get user by ID

### Product Service (http://localhost:5002/swagger)
- `POST /api/products` - Create product
- `GET /api/products` - List all products
- `GET /api/products/{id}` - Get product by ID

### Order Service (http://localhost:5003/swagger)
- `POST /api/orders` - Create order (validates products via gRPC)
- `GET /api/orders/{id}` - Get order by ID

### Payment Service (http://localhost:5004/swagger)
- `GET /api/payments/health` - Health check

### Notification Service (http://localhost:5005/swagger)
- `GET /api/notifications/health` - Health check

## Communication Flow

1. Client creates order via Order Service REST API
2. Order Service validates product existence via gRPC (Product Service)
3. Order Service saves order and publishes OrderCreatedEvent to RabbitMQ
4. Payment Service consumes OrderCreatedEvent, processes payment, publishes PaymentProcessedEvent
5. Notification Service consumes PaymentProcessedEvent and sends notification

## Project Structure

```
MicroservicesDemo/
├── src/
│   ├── Shared.EventBus/
│   ├── UserService.{Api,Application,Domain,Infrastructure}/
│   ├── ProductService.{Api,Application,Domain,Infrastructure}/
│   ├── OrderService.{Api,Application,Domain,Infrastructure}/
│   ├── PaymentService.{Api,Application,Domain,Infrastructure}/
│   ├── NotificationService.{Api,Application,Domain,Infrastructure}/
│   └── SOLID/
├── tests/
│   ├── UserService.Tests/
│   ├── ProductService.Tests/
│   ├── OrderService.Tests/
│   ├── PaymentService.Tests/
│   ├── NotificationService.Tests/
│   └── Integration.Tests/
├── docker/
│   ├── Dockerfile.userservice
│   ├── Dockerfile.productservice
│   ├── Dockerfile.orderservice
│   ├── Dockerfile.paymentservice
│   └── Dockerfile.notificationservice
├── docs/
│   ├── architecture.md
│   └── testing.md
├── docker-compose.yml
└── README.md
```

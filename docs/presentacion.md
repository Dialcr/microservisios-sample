# MicroservicesDemo — Project Presentation

## Overview

MicroservicesDemo is an educational microservices project built with **.NET 10** that demonstrates a real-world microservices architecture using modern patterns and technologies. The system simulates an e-commerce flow where orders go through a service pipeline: product validation, payment processing, and notifications.

---

## Architecture

```
                ┌──────────────┐
                │  User Service│  REST :5001
                │  (Users)     │
                └──────────────┘

                ┌──────────────┐
                │Product Service│  REST :5002
                │ (Products)    │◄──── gRPC (sync)
                └──────────────┘

                ┌──────────────┐
                │ Order Service│  REST :5003
                │  (Orders)    │────┐
                └──────────────┘    │
                                    │ RabbitMQ
                ┌──────────────┐    │ (async)
                │Payment Service│◄──┘
                │  (Payments)  │────┐
                └──────────────┘    │
                                    │ RabbitMQ
                ┌──────────────┐    │
                │ Notification │◄──┘
                │  Service     │
                └──────────────┘
```

### Communication Flow

1. **Client → Order Service** (REST): Creates an order
2. **Order Service → Product Service** (gRPC): Validates products exist
3. **Order Service → RabbitMQ**: Publishes `OrderCreatedEvent`
4. **Payment Service ← RabbitMQ**: Consumes event, processes payment
5. **Payment Service → RabbitMQ**: Publishes `PaymentProcessedEvent`
6. **Notification Service ← RabbitMQ**: Consumes event, sends notification

---

## Services

| Service | Port | Database | Purpose |
|---|---|---|---|
| **User Service** | 5001 | PostgreSQL :5432 | User CRUD |
| **Product Service** | 5002 | PostgreSQL :5433 | Product catalog + gRPC |
| **Order Service** | 5003 | PostgreSQL :5434 | Order management |
| **Payment Service** | 5004 | PostgreSQL :5435 | Payment processing |
| **Notification Service** | 5005 | PostgreSQL :5436 | Email notification |

Each service has its **own PostgreSQL database** (Database-per-service pattern).

---

## Technologies

| Technology | Usage |
|---|---|
| **.NET 10** | Base framework |
| **Clean Architecture** | Layer organization (Domain, Application, Infrastructure, API) |
| **CQRS + MediatR** | Command/Query separation |
| **Entity Framework Core** | ORM with PostgreSQL |
| **RabbitMQ** | Async inter-service communication |
| **gRPC** | Sync communication Order → Product |
| **REST + Swagger** | Documented external APIs |
| **Serilog** | Structured logging |
| **Docker Compose** | Local orchestration |
| **xUnit + FluentAssertions + Moq** | Unit and integration tests |

---

## Project Structure

```
MicroservicesDemo/
├── src/
│   ├── Shared.EventBus/           # Shared events (RabbitMQ)
│   ├── UserService.{Api,Application,Domain,Infrastructure}/
│   ├── ProductService.{Api,Application,Domain,Infrastructure}/
│   ├── OrderService.{Api,Application,Domain,Infrastructure}/
│   ├── PaymentService.{Api,Application,Domain,Infrastructure}/
│   ├── NotificationService.{Api,Application,Domain,Infrastructure}/
│   └── SOLID/                     # SOLID principle examples
├── tests/
│   ├── UserService.Tests/         # Unit tests
│   ├── ProductService.Tests/
│   ├── OrderService.Tests/
│   ├── PaymentService.Tests/
│   ├── NotificationService.Tests/
│   └── Integration.Tests/         # Integration tests
├── docker/
│   └── Dockerfile.*               # Per-service Dockerfiles
├── docs/
│   ├── architecture.md            # Architecture documentation
│   ├── testing.md                 # Testing documentation
│   └── presentacion.md            # This document
├── docker-compose.yml             # Full orchestration
└── README.md
```

---

## Clean Architecture (per service)

Each service follows **Clean Architecture** with 4 layers:

```
┌─────────────────────────────────────┐
│  Api (Controllers, Middleware)      │
│  ┌───────────────────────────────┐  │
│  │ Infrastructure (EF, RabbitMQ) │  │
│  │  ┌─────────────────────────┐  │  │
│  │  │ Application (CQRS)      │  │  │
│  │  │  ┌───────────────────┐  │  │  │
│  │  │  │ Domain (Entities)  │  │  │  │
│  │  │  └───────────────────┘  │  │  │
│  │  └─────────────────────────┘  │  │
│  └───────────────────────────────┘  │
└─────────────────────────────────────┘
```

### Domain
- Business entities (`User`, `Product`, `Order`, `Payment`, `Notification`)
- Value objects, Enums
- No external dependencies

### Application
- CQRS handlers (Commands/Queries)
- DTOs, Repository interfaces
- Depends only on Domain

### Infrastructure
- EF Core DbContext
- Concrete repositories
- RabbitMQ event bus
- **Seeders** (test data)
- Depends on Application

### API
- REST controllers
- Middleware (GlobalExceptionMiddleware)
- Configuration (Program.cs)
- Swagger / gRPC

---

## CQRS (Command Query Responsibility Segregation)

Each service implements CQRS with MediatR:

| Command | Query |
|---|---|
| `CreateUserCommand` | `GetUserByIdQuery` |
| `CreateProductCommand` | `GetProductByIdQuery`, `GetProductsQuery` |
| `CreateOrderCommand` | `GetOrderByIdQuery` |
| `ProcessPaymentCommand` | — |
| `SendNotificationCommand` | — |

**Command flow:**
```
Controller → Command → MediatR → CommandHandler → Domain → Repository → Database
```

---

## Inter-service Communication

### gRPC (synchronous)
- **Order Service → Product Service**: Product validation when creating an order
- Uses **Protocol Buffers** for binary serialization
- Defined in `src/ProductService.Api/Services/ProductGrpcService.cs`
- gRPC client in `src/OrderService.Infrastructure/Services/`

### RabbitMQ (asynchronous)
- **Published events:**
  - `OrderCreatedEvent` (Order Service → Payment Service)
  - `PaymentProcessedEvent` (Payment Service → Notification Service)
- **Exchange**: `ecommerce` (topic)
- **Routing keys**: `order.created`, `payment.processed`
- **DLQ** (Dead Letter Queue) for failed messages

---

## Seed Data

Each service includes a **seeder** that runs at startup if the database is empty, making it easy to test from Swagger.

### ProductService — Predefined Products

Used by OrderService via gRPC for validation:

| Product | ID | Price | Stock |
|---|---|---|---|
| Laptop | `33333333-...` | $1,299.99 | 15 |
| Mouse | `44444444-...` | $49.99 | 50 |
| Keyboard | `55555555-...` | $89.99 | 30 |
| Monitor | `66666666-...` | $399.99 | 20 |

### UserService — Users

| Name | Email | ID |
|---|---|---|
| John Doe | john@example.com | `11111111-...` |
| Jane Smith | jane@example.com | `22222222-...` |

### OrderService — Orders

- **Order 1**: John Doe — Laptop (1) + Mouse (2) = $1,399.97
- **Order 2**: Jane Smith — Keyboard (1) = $89.99

### PaymentService — Payments

- 2 processed payments (referencing the orders above)

### NotificationService — Notifications

- 2 sample notifications for john@example.com and jane@example.com

---

## SOLID Principles

The project includes practical examples in `src/SOLID/`:

| Principle | Description | Example |
|---|---|---|
| **SRP** | One class, one responsibility | Separate validation, persistence, email |
| **OCP** | Open for extension, closed for modification | `IDiscountStrategy` |
| **LSP** | Subtypes must be substitutable | `IShape` instead of rigid inheritance |
| **ISP** | Small, specific interfaces | `IWorkable`, `IEatable`, `ISleepable` |
| **DIP** | Depend on abstractions | `IMessageSender` injected |

---

## Tests

### Unit Tests (xUnit + Moq + FluentAssertions)
- ~15 tests covering all CQRS handlers
- Test business logic in isolation
- AAA pattern (Arrange-Act-Assert)

### Integration Tests (WebApplicationFactory)
- ~6 tests exercising real REST endpoints
- Validate full pipeline (Controller → Middleware → Handler)
- No mocking of internal dependencies

---

## How to Run

### With Docker (recommended)

```bash
docker compose up -d --build
```

Starts 5 PostgreSQL instances, RabbitMQ, and all 5 microservices.

### Local (development)

```bash
# Infrastructure only
docker compose up -d postgres-users postgres-products postgres-orders postgres-payments postgres-notifications rabbitmq

# Each service individually
cd src/UserService.Api && dotnet run --urls http://localhost:5001
cd src/ProductService.Api && dotnet run --urls http://localhost:5002
cd src/OrderService.Api && dotnet run --urls http://localhost:5003
cd src/PaymentService.Api && dotnet run --urls http://localhost:5004
cd src/NotificationService.Api && dotnet run --urls http://localhost:5005
```

### Tests

```bash
dotnet test                          # All
dotnet test tests/UserService.Tests  # Per service
```

---

## Swagger Endpoints

| Service | URL |
|---|---|
| User Service | http://localhost:5001/swagger |
| Product Service | http://localhost:5002/swagger |
| Order Service | http://localhost:5003/swagger |
| Payment Service | http://localhost:5004/swagger |
| Notification Service | http://localhost:5005/swagger |
| RabbitMQ Management | http://localhost:15672 (guest/guest) |

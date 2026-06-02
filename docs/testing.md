# Testing Documentation

## Unit Tests

Los Unit Tests validan el comportamiento de la unidad más pequeña de código de forma aislada.

### Características
- **Aislados**: No dependen de bases de datos, redes o sistemas externos
- **Rápidos**: Se ejecutan en milisegundos
- **Determinísticos**: Siempre producen el mismo resultado
- **Mockeables**: Las dependencias externas se simulan con Moq

### Patrón AAA

```
[Arrange]  Preparar datos y mocks
[Act]      Ejecutar el método bajo prueba
[Assert]   Verificar el resultado esperado
```

### Ejemplo: CreateUserCommandHandler

```csharp
[Fact]
public async Task Handle_ValidCommand_ShouldCreateUserAndReturnId()
{
    // Arrange
    var command = new CreateUserCommand("John Doe", "john@example.com");
    var handler = new CreateUserCommandHandler(mockRepo.Object);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.Should().NotBeEmpty();
    mockRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

### Tecnologías

| Herramienta | Propósito |
|-------------|-----------|
| xUnit | Framework de testing |
| FluentAssertions | Assertions legibles |
| Moq | Mocking de dependencias |

### Tests Implementados

#### UserService
- `CreateUserCommandHandler` - Crea usuario y retorna ID
- `GetUserByIdQueryHandler` - Retorna usuario existente / null si no existe

#### ProductService
- `CreateProductCommandHandler` - Crea producto con datos válidos
- `GetProductByIdQueryHandler` - Retorna producto por ID
- `GetProductsQueryHandler` - Retorna todos los productos

#### OrderService
- `CreateOrderCommandHandler` - Crea orden, valida producto vía gRPC, publica evento
- `GetOrderByIdQueryHandler` - Retorna orden por ID

#### PaymentService
- `ProcessPaymentCommandHandler` - Procesa pago y publica PaymentProcessed

#### NotificationService
- `SendNotificationCommandHandler` - Envía notificación y guarda registro

## Integration Tests

Los Integration Tests validan la interacción entre componentes reales.

### Características
- **End-to-end**: Prueban el flujo completo incluyendo middlewares
- **HTTP real**: Usan WebApplicationFactory para simular el servidor
- **Sin mocking**: Las dependencias se usan reales o con configuraciones de test

### Tecnologías
- `WebApplicationFactory<Program>` - In-memory test server
- `HttpClient` - Para hacer requests HTTP

### Tests Implementados

#### UserService API
- `CreateUser_ValidRequest_ReturnsCreated` - POST /api/users retorna 201
- `GetUser_ExistingUser_ReturnsOk` - GET /api/users/{id} retorna 200
- `GetUser_NonExistingUser_ReturnsNotFound` - GET /api/users/{id} retorna 404

#### ProductService API
- `CreateProduct_ValidRequest_ReturnsCreated` - POST /api/products retorna 201
- `GetAllProducts_ReturnsOk` - GET /api/products retorna 200

#### OrderService API
- `HealthCheck_ReturnsOk` - Verifica que el servicio responde

## Diferencia entre Unit Test e Integration Test

| Aspecto | Unit Test | Integration Test |
|---------|-----------|-----------------|
| Alcance | Unidad individual | Múltiples componentes |
| Velocidad | Milisegundos | Segundos |
| Dependencias | Mockeadas | Reales |
| Base de datos | No | Sí |
| Red | No | Sí (o simulada) |
| Confianza | Alta en la lógica | Alta en el sistema |
| Mantenimiento | Bajo | Mayor |

## Cobertura

- **Unit Tests**: ~15 tests cubriendo todos los handlers de cada servicio
- **Integration Tests**: ~6 tests cubriendo los endpoints principales REST

Para ver cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

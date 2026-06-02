# Architecture Documentation

## Microservices Architecture

Microservices Architecture es un estilo arquitectónico donde una aplicación se estructura como una colección de servicios pequeños, autónomos y débilmente acoplados.

### Ventajas
- **Independencia**: Cada servicio puede desarrollarse, desplegarse y escalarse independientemente
- **Aislamiento de fallos**: Un fallo en un servicio no afecta a los demás
- **Tecnología heterogénea**: Cada servicio puede usar diferentes tecnologías
- **Equipos pequeños**: Cada servicio puede ser mantenido por un equipo pequeño y autónomo

### Desventajas
- **Complejidad distribuida**: Mayor complejidad en redes, consistencia de datos, etc.
- **Latencia de red**: La comunicación entre servicios introduce latencia
- **Testing**: Más complejo que en monolitos

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│  User API   │    │ Product API │    │  Order API  │
│  :5001      │    │  :5002      │    │  :5003      │
└──────┬──────┘    └──────┬──────┘    └──────┬──────┘
       │                  │                  │
       ▼                  ▼                  ▼
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│ PostgreSQL  │    │ PostgreSQL  │    │ PostgreSQL  │
│  :5432      │    │  :5433      │    │  :5434      │
└─────────────┘    └─────────────┘    └──────┬──────┘
                                              │
                                              ▼
                                       ┌─────────────┐
                                       │   RabbitMQ  │
                                       │   :5672     │
                                       └──────┬──────┘
                                              │
                    ┌─────────────────────────┼──────────┐
                    │                         │          │
                    ▼                         ▼          ▼
          ┌─────────────────┐        ┌─────────────────┐
          │ Payment Service │        │ Notification    │
          │ :5004           │        │ Service :5005    │
          └────────┬────────┘        └─────────────────┘
                   │
                   ▼
          ┌─────────────────┐
          │   PostgreSQL    │
          │   :5435         │
          └─────────────────┘
```

## REST APIs

REST (Representational State Transfer) es un estilo arquitectónico para sistemas distribuidos.

### Ventajas
- **Simplicidad**: Usa HTTP estándar (GET, POST, PUT, DELETE)
- **Amplia adopción**: Todo el mundo entiende REST
- **Cacheable**: Las respuestas HTTP pueden cachearse
- **Stateless**: Cada request contiene toda la información necesaria

## RabbitMQ

### ¿Qué es un Exchange?
Un Exchange es un componente en RabbitMQ que recibe mensajes de los producers y los enruta a queues basándose en reglas definidas.

Tipos de Exchange:
- **Direct**: Enruta basado en routing key exacta
- **Topic**: Enruta basado en patrones de routing key
- **Fanout**: Enruta a todas las queues vinculadas
- **Headers**: Enruta basado en headers

### ¿Qué es una Queue?
Una Queue es un buffer que almacena mensajes hasta que un consumer los procesa. Los mensajes permanecen en la queue hasta ser consumidos (y confirmados).

### ¿Qué es una Routing Key?
La routing key es una cadena que el exchange usa para decidir cómo enrutar un mensaje a las queues.

### ¿Cuándo usar RabbitMQ?
- Comunicación asíncrona entre servicios
- Event-driven architecture
- Desacoplamiento entre producers y consumers
- Procesamiento de tareas en background
- Distribución de carga de trabajo

### Ventajas frente a REST
- **Desacoplamiento temporal**: El producer no necesita que el consumer esté disponible
- **Buffering**: Los mensajes se almacenan hasta ser procesados
- **Distribución**: Un mensaje puede ser consumido por múltiples consumers
- **Backpressure**: Las queues absorben picos de tráfico

### Dead Letter Queue (DLQ)
Una DLQ es una queue especial donde los mensajes son enviados cuando:
- El mensaje es rechazado (basic.nack o basic.reject con requeue=false)
- El TTL del mensaje expira
- La queue tiene un límite de tamaño máximo

Flujo de eventos:

```
OrderCreated ──→ exchange:ecommerce ──→ routingKey:order.created ──→ payment-order-created-queue ──→ Payment Service
                                                    │
                                                    ▼
                                         payment-order-created-dlq (DLQ)
```

## gRPC

gRPC es un framework de RPC (Remote Procedure Call) de alto rendimiento desarrollado por Google.

### HTTP/2
- Multiplexación de streams sobre una sola conexión TCP
- Bidireccional: server y client pueden enviar mensajes simultáneamente
- Headers comprimidos (menor latencia)

### Protocol Buffers
- Lenguaje de definición de interfaces (IDL)
- Serialización binaria eficiente
- Tipado fuerte
- Generación automática de código

### Beneficios frente a REST
- **Performance**: Hasta 10x más rápido que JSON
- **Tipado fuerte**: Los contratos son explícitos en .proto
- **Streaming bidireccional**: Soporte nativo
- **Generación de código**: Clientes y servidores se generan automáticamente

### Casos de uso recomendados
- Comunicación interna entre microservicios
- Sistemas de tiempo real
- Streaming de datos
- APIs de alto rendimiento

```
Order Service                    Product Service
     │                                │
     │── GetProduct(id) ──────────►   │
     │                                │
     │◄── ProductResponse ──────────  │
     │    (id, name, price, stock)    │
```

## CQRS

CQRS (Command Query Responsibility Segregation) separa las operaciones de lectura (Queries) de las de escritura (Commands).

### Commands
- Cambian el estado del sistema
- No retornan datos (solo ID)
- Validación de negocio
- Ejemplo: `CreateUserCommand`, `CreateOrderCommand`

### Queries
- No modifican el estado
- Retornan datos
- Sin efectos secundarios
- Ejemplo: `GetUserByIdQuery`, `GetProductsQuery`

### Beneficios
- **Separación de concerns**: Lectura y escritura tienen modelos diferentes
- **Escalabilidad**: Se pueden escalar lecturas y escrituras independientemente
- **Optimización**: Cada operación puede optimizarse por separado
- **Seguridad**: Se pueden aplicar políticas diferentes a lecturas y escrituras

### Flujo

```
Client ──→ Command ──→ CommandHandler ──→ Domain ──→ Repository ──→ Database
Client ──→ Query   ──→ QueryHandler   ──→ Repository ──→ Database
```

## Clean Architecture

### Capas

1. **Domain**: Entidades del negocio, Value Objects, Enums
   - Dependencias: Ninguna
   
2. **Application**: Casos de uso, DTOs, Interfaces
   - Dependencias: Domain
   
3. **Infrastructure**: Persistencia, APIs externas, Mensajería
   - Dependencias: Application
   
4. **API**: Controllers, Middleware, Program.cs
   - Dependencias: Application, Infrastructure

### Regla de Dependencia

Las dependencias apuntan hacia adentro (hacia Domain). Las capas internas no conocen las externas.

```
┌─────────────────────────────────────┐
│           API (Controllers)         │
│  ┌───────────────────────────────┐  │
│  │     Infrastructure (EF, MQ)  │  │
│  │  ┌─────────────────────────┐  │  │
│  │  │  Application (CQRS)     │  │  │
│  │  │  ┌───────────────────┐  │  │  │
│  │  │  │   Domain (Entities)│  │  │  │
│  │  │  └───────────────────┘  │  │  │
│  │  └─────────────────────────┘  │  │
│  └───────────────────────────────┘  │
└─────────────────────────────────────┘
```

## SOLID Principles

### SRP - Single Responsibility Principle
"Una clase debe tener una sola razón para cambiar"

**Before**: `UserService` maneja validación, persistencia y envío de emails.
**After**: Separamos en `UserValidator`, `UserRepository`, `EmailService`.

### OCP - Open/Closed Principle
"Las entidades deben estar abiertas para extensión, cerradas para modificación"

Implementado con `IDiscountStrategy`. Para agregar un nuevo descuento, solo creamos una nueva clase que implemente la interfaz.

### LSP - Liskov Substitution Principle
"Las subclases deben poder reemplazar a sus clases base"

**Incorrecto**: `Square` hereda de `Rectangle` rompiendo el comportamiento.
**Correcto**: Ambas implementan `IShape` de forma independiente.

### ISP - Interface Segregation Principle
"Los clientes no deben ser forzados a depender de interfaces que no usan"

**Incorrecto**: `IWorker` obliga a `RobotWorker` a implementar `Eat()` y `Sleep()`.
**Correcto**: Interfaces pequeñas (`IWorkable`, `IEatable`, `ISleepable`).

### DIP - Dependency Inversion Principle
"Depender de abstracciones, no de implementaciones concretas"

**Incorrecto**: `NotificationService` crea directamente `EmailService`.
**Correcto**: `NotificationService` depende de `IMessageSender`, inyectado desde el exterior.

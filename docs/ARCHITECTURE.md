# System Architecture

Comprehensive overview of TaskForge's architecture, design patterns, and layer responsibilities.

---

## Table of Contents

- [Overview](#overview)
- [Architectural Layers](#architectural-layers)
- [Layer Responsibilities](#layer-responsibilities)
- [Data Flow](#data-flow)
- [Dependency Inversion](#dependency-inversion)
- [Design Patterns](#design-patterns)
- [Module Organization](#module-organization)
- [ADRs & Decisions](#adrs--decisions)

---

## Overview

TaskForge implements **Clean Architecture**, ensuring:

- ✅ Independence from frameworks and UI
- ✅ Testability of business logic
- ✅ Clear separation of concerns
- ✅ Maintainability and scalability
- ✅ Flexibility for implementation changes

The architecture is organized in concentric circles where:

1. **Inner circles** = Business logic (domain-independent)
2. **Outer circles** = Technical details (frameworks, databases)
3. **Dependencies** always flow inward

---

## Architectural Layers

### Layer Hierarchy

```
┌────────────────────────────────────────────────────────────┐
│ Presentation Layer (API)                                   │
│ • HTTP Controllers                                         │
│ • Swagger/OpenAPI Documentation                            │
│ • Request/Response Handling                                │
│ Depends on: Application Layer                              │
└─────────────────────┬──────────────────────────────────────┘
                      │
┌─────────────────────▼──────────────────────────────────────┐
│ Application Layer (Use Cases)                              │
│ • Use Case Orchestration                                   │
│ • DTOs (Data Transfer Objects)                             │
│ • Business Process Flow                                    │
│ Depends on: Domain Layer                                   │
└─────────────────────┬──────────────────────────────────────┘
                      │
┌─────────────────────▼──────────────────────────────────────┐
│ Domain Layer (Business Logic)                              │
│ • Entities & Aggregates                                    │
│ • Value Objects                                            │
│ • Business Rules & Invariants                              │
│ • Repository Interfaces (contracts only)                   │
│ Depends on: Nothing (framework-independent)                │
└─────────────────────┬──────────────────────────────────────┘
                      │
┌─────────────────────▼──────────────────────────────────────┐
│ Infrastructure Layer (Implementations)                     │
│ • Repository Implementations                               │
│ • Database Context (EF Core)                               │
│ • External Service Integrations                            │
│ • Configuration & Migrations                               │
│ Depends on: Domain & Application Layers                    │
└────────────────────────────────────────────────────────────┘
```

---

## Layer Responsibilities

### 1. Presentation Layer (TaskForge.API)

**Location:** `TaskForge.API/` project

**Responsibilities:**

- Handle HTTP requests and responses
- Validate input at entry point
- Route requests to appropriate use cases
- Serialize/deserialize data
- Provide API documentation (Swagger)

**Contains:**

```
TaskForge.API/
├── Controllers/         # HTTP endpoint handlers
├── Middleware/          # Cross-cutting concerns
├── Program.cs           # DI setup & configuration
└── Dockerfile           # Container configuration
```

**Does NOT contain:**

- ❌ Business logic
- ❌ Database access
- ❌ Direct domain object manipulation

**Key Pattern:** Dependency Injection

---

### 2. Application Layer (TaskForge.Application)

**Location:** `TaskForge.Application/` project

**Responsibilities:**

- Orchestrate use cases
- Coordinate domain objects
- Manage application workflows
- Transform domain objects to DTOs
- Handle transactional boundaries

**Contains:**

```
TaskForge.Application/
├── UseCases/           # Use case implementations
│   ├── CreateUserUseCase.cs
│   ├── GetUsersUseCase.cs
│   └── ...
├── DTOs/               # Data transfer objects
│   ├── CreateUserRequest.cs
│   ├── UserResponse.cs
│   └── ...
└── Interfaces/         # Contracts from domain
```

**Example Use Case:**

```csharp
public class CreateUserUseCase
{
    private readonly IUserRepository _repository;

    public CreateUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserResponse> ExecuteAsync(CreateUserRequest request)
    {
        // Coordinate domain operations
        var user = User.Create(request.Email, request.Name);
        await _repository.AddAsync(user);
        return new UserResponse { Id = user.Id, ... };
    }
}
```

**Does NOT contain:**

- ❌ HTTP details
- ❌ EF Core configurations
- ❌ Deep business logic (belongs in Domain)

---

### 3. Domain Layer (TaskForge.Domain)

**Location:** `TaskForge.Domain/` project

**Responsibilities:**

- Encapsulate business rules
- Protect domain invariants
- Define entity behavior
- Provide repository contracts
- Remain framework-independent

**Contains:**

```
TaskForge.Domain/
├── Entities/           # Domain entities
│   ├── User.cs
│   └── ...
├── ValueObjects/       # Immutable value objects
│   ├── Email.cs
│   ├── Password.cs
│   └── ...
├── Interfaces/         # Repository contracts
│   ├── IUserRepository.cs
│   └── ...
└── Services/           # Domain services (optional)
```

**Example Entity:**

```csharp
public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }

    // Factory method - ensures valid creation
    public static User Create(string email, string name)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");

        return new User
        {
            Id = Guid.NewGuid(),
            Email = new Email(email),
            Name = name
        };
    }

    // Behavior - domains objects do things
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Name cannot be empty");
        Name = newName;
    }
}
```

**Does NOT contain:**

- ❌ Entity Framework references
- ❌ HTTP dependencies
- ❌ Database-specific code
- ❌ External service calls

---

### 4. Infrastructure Layer (TaskForge.Infrastructure)

**Location:** `TaskForge.Infrastructure/` project

**Responsibilities:**

- Implement repository interfaces
- Configure database mapping
- Handle entity persistence
- Execute migrations
- Integrate external services

**Contains:**

```
TaskForge.Infrastructure/
├── Context/            # EF Core context
│   ├── AppDbContext.cs
│   └── Configuration/  # Entity configurations
├── Repositories/       # Repository implementations
│   ├── UserRepository.cs
│   └── ...
├── Migrations/         # Database migrations
│   ├── 20260416003130_InitialCreate.cs
│   └── ...
└── ExternalServices/   # 3rd party integrations
```

**Example Repository:**

```csharp
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
```

**Does NOT contain:**

- ❌ Business rules (belong in Domain)
- ❌ Orchestration logic (belongs in Application)
- ❌ HTTP handling (belongs in API)

---

## Data Flow

### Request Flow (Inbound)

```
1. HTTP Request arrives
   ↓
2. Controller receives and validates
   ↓
3. Controller calls Use Case
   ↓
4. Use Case orchestrates Domain objects
   ↓
5. Use Case calls Repository interface
   ↓
6. Repository (Infrastructure) accesses database
   ↓
7. Returns Entity to Use Case
   ↓
8. Use Case transforms to DTO
   ↓
9. Controller returns HTTP Response
```

### Example Request Journey

```
POST /api/users
{
  "email": "john@example.com",
  "name": "John Doe"
}
  ↓ API Layer
CreateUserController.Post()
  ↓ Application Layer
CreateUserUseCase.ExecuteAsync()
  ↓ Domain Layer
User.Create(email, name)
  ↓ Infrastructure Layer
UserRepository.AddAsync(user)
  ↓ Database
INSERT INTO Users ...
  ↓
Response: 201 Created
```

---

## Dependency Inversion

### Key Principle

> High-level modules should not depend on low-level modules. Both should depend on abstractions.

### Implementation

**Domain defines interfaces:**
```csharp
// In TaskForge.Domain
public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task AddAsync(User user);
}
```

**Infrastructure implements:**
```csharp
// In TaskForge.Infrastructure
public class UserRepository : IUserRepository
{
    // Implementation using EF Core
}
```

**Application uses abstraction:**
```csharp
// In TaskForge.Application
public class CreateUserUseCase
{
    private readonly IUserRepository _repository; // Depends on interface
    
    public CreateUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }
}
```

**Benefit:** Domain remains independent of infrastructure details.

---

## Design Patterns

### 1. Repository Pattern

Abstracts data access, allowing domain to remain database-agnostic.

```csharp
// Contract in Domain
public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
}

// Implementation in Infrastructure
public class UserRepository : IUserRepository
{
    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }
}
```

### 2. Dependency Injection

Loosely couples components; enables testing.

```csharp
// In Program.cs
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<CreateUserUseCase>();
```

### 3. Entity Pattern

Domain objects encapsulate behavior and maintain invariants.

```csharp
public class User
{
    public void UpdateEmail(Email newEmail)
    {
        // Business rule enforcement
        Email = newEmail;
    }
}
```

### 4. Value Object Pattern

Immutable objects representing domain concepts without identity.

```csharp
public class Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException("Invalid email");
        Value = value;
    }
}
```

### 5. DTO Pattern

Separates internal domain model from external API contract.

```csharp
// Domain Entity
public class User { /* ... */ }

// API DTO
public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
}
```

---

## Module Organization

### Project Structure

```
TaskForge.sln
├── TaskForge.API/
│   └── Controllers/
│       └── UsersController.cs
├── TaskForge.Application/
│   ├── UseCases/
│   │   └── CreateUserUseCase.cs
│   └── DTOs/
│       └── CreateUserRequest.cs
├── TaskForge.Domain/
│   ├── Entities/
│   │   └── User.cs
│   └── Interfaces/
│       └── IUserRepository.cs
└── TaskForge.Infrastructure/
    ├── Repositories/
    │   └── UserRepository.cs
    └── Context/
        └── AppDbContext.cs
```

### Naming Conventions

| Layer | Pattern | Example |
|-------|---------|---------|
| API | `{Entity}Controller` | `UsersController` |
| Application | `{Action}{Entity}UseCase` | `CreateUserUseCase` |
| Application | `{Entity}Request/Response` | `CreateUserRequest` |
| Domain | `{Entity}` | `User` |
| Domain | `I{Entity}Repository` | `IUserRepository` |
| Infrastructure | `{Entity}Repository` | `UserRepository` |

---

## ADRs & Decisions

Each architectural layer has its own Architecture Decision Record:

- **[Domain Layer ADR](Domain/1.Camada%20Domain.md)** — Core business logic isolation
- **[Application Layer ADR](Application/2.Camada%20Application.md)** — Use case orchestration
- **[API Layer ADR](API/3.Camada%20API.md)** — HTTP interface design
- **[Infrastructure Layer ADR](Infrastructure/4.Camada%20Infraestrutura.md)** — Technical implementation

---

## Scalability Considerations

### Future Growth Patterns

- **Event Sourcing:** Store state as domain events
- **CQRS:** Separate read/write models
- **Microservices:** Split layers into independent services
- **Domain-Driven Design:** Define bounded contexts

### Current Foundation

The current architecture supports these patterns through:
- Clear layer separation
- Dependency inversion
- Repository abstraction
- Use case isolation

---

## Resources

- [Clean Architecture Book](https://www.oreilly.com/library/view/clean-architecture/9780134494272/)
- [Domain-Driven Design](https://www.oreilly.com/library/view/domain-driven-design/9780321125675/)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

**Architecture Last Updated:** 2026-04-16

# ADR-003 — Application Layer Architecture

**Author:** Isabelle Bicudo  
**Date:** 2026-04-16  
**Status:** Accepted  
**Related:** [Domain Layer](../Domain/) | [Architecture Overview](../ARCHITECTURE.md)

---

## Executive Summary

The Application layer orchestrates complex use cases by coordinating domain objects and infrastructure services. It translates user requests into domain operations while remaining independent of HTTP or framework specifics.

---

## Problem

Without a dedicated Application layer:

- Controllers accumulate orchestration logic
- Business workflows become implicit
- Logic cannot be reused from other interfaces (CLI, jobs, events)
- Testing requires HTTP or framework setup

---

## Decision

Create an **Application Layer** as an explicit orchestration layer:

> A thin layer that coordinates use cases by orchestrating domain operations,
> applying infrastructure, and transforming data for external interfaces

---

## Structure

### Project Organization

```
TaskForge.Application/
├── UseCases/           # Business workflows
│   ├── Users/
│   │   ├── RegisterUserUseCase.cs
│   │   └── GetUserByIdUseCase.cs
│   └── Tasks/
├── DTOs/               # Data transfer objects
│   ├── Requests/
│   │   └── RegisterUserRequest.cs
│   └── Responses/
│       └── UserResponse.cs
├── Mappers/            # Domain ↔ DTO transformation
│   └── UserMapper.cs
└── Validators/         # Input validation
    └── RegisterUserValidator.cs
```

### Contains

✅ Use cases (business workflows)  
✅ DTOs (request/response contracts)  
✅ Mappers (domain ↔ DTO)  
✅ Application services (orchestration)  
✅ Input validators  

### Does NOT Contain

❌ HTTP/REST concerns  
❌ Entity Framework configurations  
❌ Deep business logic  
❌ Infrastructure implementations  

---

## Implementation Patterns

### Use Case

```csharp
public class RegisterUserUseCase
{
    private readonly IUserRepository _repository;
    private readonly IEmailService _emailService;

    public RegisterUserUseCase(
        IUserRepository repository,
        IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }

    public async Task<UserResponse> ExecuteAsync(RegisterUserRequest request)
    {
        // Validate input
        if (await _repository.ExistsByEmailAsync(request.Email))
            throw new ApplicationException("Email already registered");

        // Use domain for business logic
        var user = User.Create(request.Name, request.Email);

        // Use infrastructure
        await _repository.AddAsync(user);
        await _emailService.SendVerificationEmailAsync(user.Email.Value);

        // Return DTO
        return UserMapper.ToResponse(user);
    }
}
```

### DTOs

```csharp
// Request
public class RegisterUserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// Response
public class UserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```

### Mapper

```csharp
public static class UserMapper
{
    public static UserResponse ToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email.Value
        };
    }
}
```

---

## Workflow

```
Request → Validate → Domain Logic → Infrastructure → Transform → Response
  ↓         ↓          ↓              ↓               ↓           ↓
 API       UseCase    Entity       Repository        DTO        HTTP
```

---

## Consequences

### Positive

✅ Reusable from any interface (API, CLI, jobs)  
✅ Testable without HTTP setup  
✅ Explicit, understandable workflows  
✅ Maintainable: changes in one place  
✅ API evolution independent of domain  
✅ Security: DTOs prevent data leakage  

### Negative

❌ More classes and interfaces  
❌ Boilerplate (DTOs, mappers)  
❌ Learning curve  

---

## References

- [Use Case Pattern - Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Data Transfer Objects](https://martinfowler.com/eaaCatalog/dataTransferObject.html)

---

**Status:** Accepted | **Last Reviewed:** 2026-04-16

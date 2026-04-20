# ADR-002 — Domain Layer Architecture

**Author:** Isabelle Bicudo  
**Date:** 2026-04-16  
**Status:** Accepted  
**Related:** [Application Layer](../Application/) | [Architecture Overview](../ARCHITECTURE.md)

---

## Executive Summary

The Domain layer encapsulates all business logic independent of any framework or technology. It remains the core of the system, protected from infrastructure details.

---

## Problem

Without a properly isolated domain layer:

- Business rules scatter across controllers and services
- Logic becomes dependent on Entity Framework Core
- Code loses reusability potential
- Unit testing requires complex mocking of infrastructure
- Changing frameworks requires rewriting business logic

---

## Decision

Create a **Domain Layer** as the core of the system containing:

> Pure business logic, independent of frameworks, providing the foundation for all application behavior

**Characteristics:**
- No dependencies on external frameworks
- No references to Entity Framework, ASP.NET Core, or HTTP
- Highly testable with simple unit tests
- Framework-agnostic implementations

---

## Structure

### Project Organization

```
TaskForge.Domain/
├── Entities/           # Domain entities with identity
│   └── User.cs
├── ValueObjects/       # Immutable, identity-less concepts
│   ├── Email.cs
│   └── Password.cs
├── Interfaces/         # Repository contracts (definitions only)
│   └── IUserRepository.cs
├── Exceptions/         # Domain-specific exceptions
│   └── DomainException.cs
└── Services/           # Complex cross-entity business logic
```

### Contains

✅ Entities with business behavior  
✅ Value objects with validation  
✅ Repository interfaces (contracts)  
✅ Domain exceptions  
✅ Business rules and invariants  

### Does NOT Contain

❌ Entity Framework Core  
❌ ASP.NET Core  
❌ HTTP concerns  
❌ Repository implementations  
❌ Database configurations  

---

## Implementation Patterns

### Entity Pattern

```csharp
public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }

    // Factory method ensures valid construction
    public static User Create(string name, string email)
    {
        if (string.IsNullOrEmpty(name))
            throw new DomainException("Name required");
        
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = new Email(email)
        };
    }

    // Behavior - entities DO things
    public void UpdateName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
            throw new DomainException("Name cannot be empty");
        Name = newName;
    }

    private User() { } // For ORM only
}
```

### Value Object Pattern

```csharp
public class Email : IEquatable<Email>
{
    public string Value { get; }

    public Email(string value)
    {
        if (!IsValid(value))
            throw new DomainException("Invalid email");
        Value = value.ToLowerInvariant();
    }

    // Value objects compare by value
    public bool Equals(Email other) =>
        other != null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    private static bool IsValid(string email) =>
        email?.Contains("@") == true;
}
```

### Repository Interface

```csharp
public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}
```

---

## Consequences

### Positive

✅ Framework-independent business logic  
✅ Testable without database or HTTP  
✅ Reusable from any interface (API, CLI, jobs)  
✅ Clear separation of concerns  
✅ Easy to maintain and extend  

### Negative

❌ More abstractions initially  
❌ Steeper learning curve  
❌ Additional classes to maintain  

---

## References

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design - Eric Evans](https://www.oreilly.com/library/view/domain-driven-design/9780321125675/)

---

**Status:** Accepted | **Last Reviewed:** 2026-04-16

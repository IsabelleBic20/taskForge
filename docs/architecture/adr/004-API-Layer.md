# ADR-004 — API Layer Architecture

**Author:** Isabelle Bicudo  
**Date:** 2026-04-16  
**Status:** Accepted  
**Related:** [Application Layer](../Application/) | [Architecture Overview](../ARCHITECTURE.md)

---

## Executive Summary

The API layer provides HTTP entry points to the system. It handles REST concerns and delegates business logic to the application layer.

---

## Problem

Without a clean API layer:

- Controllers mix HTTP concerns with business logic
- Business logic becomes HTTP-specific
- Reusing logic from other interfaces becomes impossible
- Hard to version APIs without affecting implementation

---

## Decision

Create an **API Layer** as the HTTP interface:

> Controllers receive HTTP requests, validate entry point concerns, delegate to use cases,
> and return HTTP responses

---

## Structure

### Project Organization

```
TaskForge.API/
├── Controllers/        # HTTP endpoint handlers
│   ├── UsersController.cs
│   └── HealthController.cs
├── Middleware/         # Request/response middleware
│   ├── ExceptionHandlingMiddleware.cs
│   └── LoggingMiddleware.cs
├── Program.cs          # DI configuration & startup
├── Dockerfile          # Container configuration
└── TaskForge.API.csproj
```

### Contains

✅ Controllers (HTTP endpoints)  
✅ Middleware (cross-cutting concerns)  
✅ Dependency injection setup  
✅ Configuration  
✅ Swagger/OpenAPI documentation  

### Does NOT Contain

❌ Business logic  
❌ Database access  
❌ Domain object manipulation  

---

## Implementation Pattern

### Controller

```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUseCase;
    private readonly GetUserByIdUseCase _getUseCase;

    public UsersController(
        RegisterUserUseCase registerUseCase,
        GetUserByIdUseCase getUseCase)
    {
        _registerUseCase = registerUseCase;
        _getUseCase = getUseCase;
    }

    /// <summary>Register a new user</summary>
    [HttpPost("register")]
    [ProduceResponseType(StatusCodes.Status201Created)]
    [ProduceResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request)
    {
        var response = await _registerUseCase.ExecuteAsync(request);
        return Created(nameof(Register), response);
    }

    /// <summary>Get user by ID</summary>
    [HttpGet("{id}")]
    [ProduceResponseType(StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _getUseCase.ExecuteAsync(id);
        return Ok(response);
    }
}
```

### Dependency Injection

```csharp
// Program.cs
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add domain/application services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<GetUserByIdUseCase>();

// Add API services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Configure middleware
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
```

---

## Responsibilities

✅ HTTP request routing  
✅ Input validation (HTTP format)  
✅ Status code selection  
✅ Response serialization  
✅ Error handling  
✅ Swagger documentation  

---

## Principles

- **Single Responsibility:** Each endpoint does one thing
- **Stateless:** No state between requests
- **Documented:** Swagger comments on all endpoints
- **Validated:** Input validation before business logic
- **Clear Status Codes:** Correct HTTP responses

---

## Consequences

### Positive

✅ Clear HTTP interface  
✅ Easy to understand endpoints  
✅ Self-documenting with Swagger  
✅ Testable with HTTP client  
✅ Follows REST conventions  

### Negative

❌ Additional layer of routing  
❌ Swagger requires documentation  

---

## References

- [ASP.NET Core Routing](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing)
- [REST API Best Practices](https://restfulapi.net/)
- [OpenAPI Specification](https://www.openapis.org/)

---

**Status:** Accepted | **Last Reviewed:** 2026-04-16

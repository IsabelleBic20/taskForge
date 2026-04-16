# ADR-005 — Infrastructure Layer Architecture

**Author:** Isabelle Bicudo  
**Date:** 2026-04-16  
**Status:** Accepted  
**Related:** [Domain Layer](../Domain/) | [Architecture Overview](../ARCHITECTURE.md)

---

## Executive Summary

The Infrastructure layer implements technical details and external integrations. It implements domain contracts without exposing infrastructure details to higher layers.

---

## Problem

Without a clean Infrastructure layer:

- Database technology leaks into domain logic
- Changing ORM requires rewriting business logic
- Infrastructure concerns mix with domain rules
- Testing requires complex database setup

---

## Decision

Create an **Infrastructure Layer** responsible for:

> Implementation of data access, external services, and technical details.
> Hides infrastructure complexity behind domain contracts.

---

## Structure

### Project Organization

```
TaskForge.Infrastructure/
├── Context/            # Entity Framework DbContext
│   ├── AppDbContext.cs
│   └── Configuration/  # Entity configurations
│       └── UserConfiguration.cs
├── Repositories/       # Repository implementations
│   └── UserRepository.cs
├── Migrations/         # Database migrations
│   ├── 20260416_InitialCreate.cs
│   └── AppDbContextModelSnapshot.cs
└── ExternalServices/   # 3rd party integrations
    └── EmailService.cs
```

### Contains

✅ Repository implementations  
✅ Entity Framework context  
✅ Entity configurations  
✅ Database migrations  
✅ External service implementations  

### Does NOT Contain

❌ Business rules  
❌ Application orchestration  
❌ HTTP concerns  

---

## Implementation Patterns

### Repository Implementation

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

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
```

### Entity Configuration

```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        // Map value object
        builder.OwnsOne(u => u.Email, nav =>
        {
            nav.Property(e => e.Value)
                .HasColumnName("Email");
        });
    }
}
```

### DbContext

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}
```

---

## Migration Strategy

### Create Migration

```bash
dotnet ef migrations add AddUserTable \
    -p TaskForge.Infrastructure \
    -s TaskForge.API
```

### Apply Migration

```bash
dotnet ef database update \
    -p TaskForge.Infrastructure \
    -s TaskForge.API
```

### Generated Migration

```csharp
public partial class AddUserTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(),
                Name = table.Column<string>(maxLength: 100),
                Email = table.Column<string>(maxLength: 255)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("Users");
    }
}
```

---

## Responsibilities

✅ Data persistence (repositories)  
✅ Database mapping (configurations)  
✅ External service integrations  
✅ Migration management  
✅ Technical implementation details  

---

## Principles

- **Single Responsibility:** Each repository handles one entity type
- **Loose Coupling:** Use domain interfaces, not concrete types
- **Consistency:** Follow patterns for all repositories
- **Encapsulation:** Hide EF Core complexity
- **Testability:** Repositories can be mocked

---

## Consequences

### Positive

✅ Business logic independent of database  
✅ Technology swaps without business impact  
✅ Easy to mock for testing  
✅ Clear separation of concerns  
✅ Database optimization isolated  

### Negative

❌ Mapping configuration required  
❌ Value object mapping complexity  
❌ Migration management overhead  

---

## References

- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)
- [Data Mapper Pattern](https://martinfowler.com/eaaCatalog/dataMapper.html)

---

**Status:** Accepted | **Last Reviewed:** 2026-04-16

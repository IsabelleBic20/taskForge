# TaskForge

> A backend system for task management built with .NET 8 and Clean Architecture.

TaskForge is a portfolio project demonstrating professional software architecture, clean code principles, and domain-driven design. It includes Docker containerization, SQL Server integration, and comprehensive architectural documentation.

**Status:** In Development | **Version:** 1.0.0-alpha | **License:** MIT

---

## Why This Project?

This project showcases:

- **Clean Architecture** — Clear separation between layers (API, Application, Domain, Infrastructure)
- **Domain-Driven Design** — Business logic isolated with entities and value objects
- **Professional Patterns** — Repository pattern, dependency injection, SOLID principles
- **Real-World Setup** — Docker, EF Core, SQL Server, Swagger
- **Architectural Thinking** — Documented decisions via Architecture Decision Records (ADRs)

Built to demonstrate how backend systems should be structured for scalability, testability, and maintainability.

---

## Quick Start

```bash
# 1. Clone
git clone <repository-url> && cd taskforge

# 2. Start services
docker compose up --build

# 3. Apply migrations
dotnet ef database update -p TaskForge.Infrastructure -s TaskForge.API

# 4. Access API
# http://localhost:5000/swagger
```

---

## Architecture

Clean Architecture with 4 layers:

```
API Layer (HTTP endpoints)
    ↓
Application Layer (Use cases, orchestration)
    ↓
Domain Layer (Business logic, entities)
    ↓
Infrastructure Layer (Data access, persistence)
```

**See:** [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for detailed explanation.

---

## Tech Stack

- .NET 8, ASP.NET Core Web API
- Entity Framework Core, SQL Server
- Docker, Docker Compose
- Swagger (OpenAPI 3.0)
- C# + Clean Architecture

---

## Project Structure

```
taskforge/
├── TaskForge.API/                # HTTP controllers
├── TaskForge.Application/        # Use cases
├── TaskForge.Domain/             # Entities & business rules
├── TaskForge.Infrastructure/     # Data access
└── docs/
    ├── ARCHITECTURE.md
    ├── DEVELOPMENT.md
    └── adr/                      # Architecture Decision Records
        ├── 002-Domain-Layer.md
        ├── 003-Application-Layer.md
        ├── 004-API-Layer.md
        └── 005-Infrastructure-Layer.md
```

---

## Documentation

- **[ARCHITECTURE.md](docs/ARCHITECTURE.md)** — System design and layers
- **[DEVELOPMENT.md](docs/DEVELOPMENT.md)** — Setup, workflows, troubleshooting
- **[CONTRIBUTING.md](CONTRIBUTING.md)** — Code standards and contribution
- **[ADR-002: Domain Layer](docs/adr/002-Domain-Layer.md)** — Domain layer design
- **[ADR-003: Application Layer](docs/adr/003-Application-Layer.md)** — Application layer design
- **[ADR-004: API Layer](docs/adr/004-API-Layer.md)** — API layer design
- **[ADR-005: Infrastructure Layer](docs/adr/005-Infrastructure-Layer.md)** — Infrastructure layer design

---

## Common Commands

| Task | Command |
|------|---------|
| Build | `dotnet build` |
| Test | `dotnet test` |
| Run | `dotnet run -p TaskForge.API` |
| Migration | `dotnet ef migrations add NAME -p TaskForge.Infrastructure -s TaskForge.API` |
| Database | `dotnet ef database update -p TaskForge.Infrastructure -s TaskForge.API` |
| Docker | `docker compose up --build` |

---

## Key Features

- Clean Architecture with layer separation
- Domain-driven design
- Dependency injection & loose coupling
- Repository pattern
- Docker containerization
- EF Core with SQL Server
- Swagger documentation
- Professional ADRs

---

## Setup

**Prerequisites:** .NET 8.0+, Docker, Git

**See [DEVELOPMENT.md](docs/DEVELOPMENT.md) for:**
- OS-specific setup (Windows/Mac/Linux)
- Development workflows
- Database management
- Troubleshooting

---

## License

MIT

---

**Last Updated:** 2026-04-16

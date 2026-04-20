# Development Guide

Comprehensive guide for setting up your development environment and working with TaskForge.

---

## Table of Contents

- [Environment Setup](#environment-setup)
- [Project Structure](#project-structure)
- [Workflow](#workflow)
- [Database Management](#database-management)
- [Testing](#testing)
- [Debugging](#debugging)
- [Common Problems](#common-problems)
- [Performance Tips](#performance-tips)

---

## Environment Setup

### System Requirements

| Component | Requirement |
|-----------|------------|
| **OS** | Windows / macOS / Linux |
| **.NET SDK** | 8.0 or higher |
| **Docker** | Latest stable version |
| **Docker Compose** | v2.0+ |
| **Git** | Latest version |
| **Editor** | Visual Studio 2022+ OR VS Code + C# DevKit |

### Installation Steps

#### 1. Install Prerequisites

**Windows (PowerShell as Admin):**
```powershell
# Using Chocolatey
choco install dotnet-sdk docker-desktop git

# Verify
dotnet --version
docker --version
git --version
```

**macOS (using brew):**
```bash
brew install dotnet docker git
docker --version
```

**Linux (Ubuntu/Debian):**
```bash
# .NET
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version latest

# Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Verify
dotnet --version
docker --version
```

#### 2. Clone Repository

```bash
git clone https://github.com/your-org/taskforge.git
cd taskforge
```

#### 3. Configure Environment Variables

Create `.env` file in project root:

```env
TASKFORGE_DB=TaskForgeDb
SA_PASSWORD=Your_password123
ASPNETCORE_ENVIRONMENT=Development
```

#### 4. Start Services

```bash
docker compose up --build
```

#### 5. Apply Migrations

```bash
dotnet ef database update -p TaskForge.Infrastructure -s TaskForge.API
```

#### 6. Verify Setup

```bash
# API health check
curl http://localhost:5000/health

# Swagger UI
# Open: http://localhost:5000/swagger
```

---

## Project Structure

### Clean Architecture Layout

```
taskforge/
├── .github/                    # GitHub workflows & configurations
├── docs/                       # Documentation
│   ├── ARCHITECTURE.md         # Architecture overview (start here!)
│   ├── QUICK_START.md          # 5-minute setup
│   ├── Domain/                 # Domain layer ADR
│   ├── Application/            # Application layer ADR
│   ├── API/                    # API layer ADR
│   └── Infrastructure/         # Infrastructure layer ADR
│
├── taskforge/                  # .NET Solution
│   ├── TaskForge.sln           # Solution file
│   │
│   ├── TaskForge.API/          # 🔵 Presentation Layer
│   │   ├── Controllers/        # HTTP endpoints
│   │   ├── Middleware/         # Middleware components
│   │   ├── Program.cs          # Application entry point & DI
│   │   ├── Dockerfile          # Container configuration
│   │   └── TaskForge.API.csproj
│   │
│   ├── TaskForge.Application/  # 🟢 Application Layer
│   │   ├── UseCases/           # Use case implementations
│   │   │   └── CreateUserUseCase.cs
│   │   ├── DTOs/               # Data transfer objects
│   │   │   ├── CreateUserRequest.cs
│   │   │   └── UserResponse.cs
│   │   └── TaskForge.Application.csproj
│   │
│   ├── TaskForge.Domain/       # 🟡 Domain Layer
│   │   ├── Entities/           # Domain entities
│   │   │   └── User.cs
│   │   ├── ValueObjects/       # Value objects (immutable)
│   │   ├── Interfaces/         # Repository contracts
│   │   │   └── IUserRepository.cs
│   │   └── TaskForge.Domain.csproj
│   │
│   └── TaskForge.Infrastructure/ # 🔴 Infrastructure Layer
│       ├── Context/            # Entity Framework DbContext
│       ├── Repositories/       # Repository implementations
│       │   └── UserRepository.cs
│       ├── Migrations/         # Database migrations
│       │   └── 20260416003130_InitialCreate.cs
│       └── TaskForge.Infrastructure.csproj
│
├── docker-compose.yml          # Service orchestration
├── CONTRIBUTING.md             # Contribution guidelines
├── README.md                   # Project overview
└── .gitignore                  # Git ignore rules
```

### Building a New Feature

Example: Adding a "Task" feature

```
1. Create Domain Entity
   └── TaskForge.Domain/Entities/Task.cs

2. Create Repository Interface
   └── TaskForge.Domain/Interfaces/ITaskRepository.cs

3. Create Application DTOs
   └── TaskForge.Application/DTOs/CreateTaskRequest.cs
   └── TaskForge.Application/DTOs/TaskResponse.cs

4. Create Use Case
   └── TaskForge.Application/UseCases/CreateTaskUseCase.cs

5. Create Repository Implementation
   └── TaskForge.Infrastructure/Repositories/TaskRepository.cs

6. Add DbSet to Context
   └── TaskForge.Infrastructure/Context/AppDbContext.cs

7. Create Migration
   └── TaskForge.Infrastructure/Migrations/AddTaskEntity.cs

8. Create API Controller
   └── TaskForge.API/Controllers/TasksController.cs

9. Register in DI
   └── TaskForge.API/Program.cs
```

---

## Workflow

### Daily Development Loop

#### Start of Day

```bash
# Update from main branch
git fetch origin
git rebase origin/main

# Install any new dependencies
dotnet restore

# Start services
docker compose up --build
```

#### During Work

```bash
# Make incremental commits
git add .
git commit -m "feat(user): add email validation"

# Local testing
dotnet build
dotnet test

# Push changes
git push origin feature-branch
```

#### End of Day

```bash
# Ensure clean working directory
git status

# Stop services
docker compose down

# Commit any WIP changes
git commit -m "wip: temp commit"
```

### Create a Feature Branch

```bash
# From main branch, create feature branch
git checkout main
git pull origin main
git checkout -b feature/user-authentication

# Example branch names:
# feature/user-registration
# fix/email-validation
# docs/api-authentication
# test/user-service
```

### Making Commits

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```bash
# Feature
git commit -m "feat(user): add password hashing"

# Bug fix
git commit -m "fix(api): handle null email in response"

# Documentation
git commit -m "docs(architecture): clarify domain layer"

# Testing
git commit -m "test(domain): add email validation tests"

# Refactoring
git commit -m "refactor(repository): extract common query logic"
```

### Create a Pull Request

```bash
# Push to origin
git push origin feature/your-feature

# Create PR on GitHub
# - Use template
# - Request reviewers
# - Link related issues
# - Ensure CI passes
```

---

## Database Management

### Entity Framework Core Commands

| Operation | Command |
|-----------|---------|
| Add migration | `dotnet ef migrations add MigrationName -p TaskForge.Infrastructure -s TaskForge.API` |
| List migrations | `dotnet ef migrations list -p TaskForge.Infrastructure` |
| Remove last migration | `dotnet ef migrations remove -p TaskForge.Infrastructure` |
| Update database | `dotnet ef database update -p TaskForge.Infrastructure -s TaskForge.API` |
| Update to specific migration | `dotnet ef database update MigrationName -p TaskForge.Infrastructure -s TaskForge.API` |
| Drop database | `dotnet ef database drop -p TaskForge.Infrastructure -s TaskForge.API` |
| Create script | `dotnet ef migrations script -p TaskForge.Infrastructure -s TaskForge.API > migration.sql` |

### Migration Workflow

#### Create a Migration

```bash
# 1. Make entity changes
# Edit: TaskForge.Domain/Entities/User.cs

# 2. Add migration
dotnet ef migrations add AddUserNameField -p TaskForge.Infrastructure -s TaskForge.API

# 3. Verify generated migration
# Review: TaskForge.Infrastructure/Migrations/20260416_AddUserNameField.cs

# 4. Apply migration
dotnet ef database update -p TaskForge.Infrastructure -s TaskForge.API
```

#### Undo a Migration

```bash
# Remove last migration (not yet applied)
dotnet ef migrations remove -p TaskForge.Infrastructure

# Revert applied migration
dotnet ef database update 20260415_PreviousMigration -p TaskForge.Infrastructure
```

### Database Connection

**Development (Docker):**
```
Server=db
Database=TaskForgeDb
User Id=sa
Password=Your_password123
TrustServerCertificate=True
```

**Local (if running SQL Server directly):**
```
Server=localhost,1433
Database=TaskForgeDb
User Id=sa
Password=Your_password123
TrustServerCertificate=True
```

### SQL Server Management

Connect with SQL Server Management Studio or VS Code SQL Server extension:

```bash
# Server: localhost,1433
# Username: sa
# Password: Your_password123
# Leave Database blank (auto-selects default)
```

---

## Testing

### Project Structure

```
TaskForge.Tests/                   # Main test project
├── Domain/                        # Domain layer tests
│   └── Entities/
│       └── UserTests.cs
├── Application/                   # Application layer tests
│   └── UseCases/
│       └── CreateUserUseCaseTests.cs
└── API/                          # API integration tests
    ├── Controllers/
    │   └── UsersControllerTests.cs
    └── Fixtures/
        └── TestDatabaseFixture.cs
```

### Writing Tests

#### Unit Test Example

```csharp
using Xunit;
using TaskForge.Domain.Entities;

namespace TaskForge.Tests.Domain.Entities
{
    public class UserTests
    {
        [Fact]
        public void Create_WithValidEmail_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var name = "John Doe";

            // Act
            var user = User.Create(email, name);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(email, user.Email.Value);
            Assert.Equal(name, user.Name);
        }

        [Fact]
        public void Create_WithInvalidEmail_ThrowsException()
        {
            // Arrange
            var invalidEmail = "invalid-email";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                User.Create(invalidEmail, "John"));
        }
    }
}
```

#### Integration Test Example

```csharp
using Xunit;
using TaskForge.Application.UseCases;
using TaskForge.Application.DTOs;

public class CreateUserUseCaseTests : IAsyncLifetime
{
    private readonly CreateUserUseCase _useCase;

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsUserResponse()
    {
        // Arrange
        var request = new CreateUserRequest 
        { 
            Email = "user@example.com",
            Name = "John Doe"
        };

        // Act
        var response = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("user@example.com", response.Email);
    }
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test TaskForge.Tests.Domain

# Run specific test class
dotnet test --filter ClassName=UserTests

# Run with coverage
dotnet test /p:CollectCoverageMetrics=true

# Watch mode (auto-run on changes)
dotnet watch test
```

---

## Debugging

### Visual Studio 2022

1. **Set breakpoints:** Click left margin in code editor
2. **Start debugging:** F5 or Debug menu
3. **Step through:** F10 (step over), F11 (step into)
4. **View variables:** Hover over variables or use Watch window
5. **Conditional breaks:** Right-click breakpoint → Filter

### VS Code

1. **Install C# DevKit**
2. **Set breakpoints:** Click left margin
3. **Start debugging:** F5 or Run menu
4. **View Debug Console:** Ctrl+Shift+Y

### Docker Debugging

```bash
# Connect to running container
docker compose exec api bash

# View live logs
docker compose logs -f api

# Execute command in container
docker compose exec api dotnet test
```

### Common Debugging Scenarios

**API not responding:**
```bash
# Check if container is running
docker ps

# View API logs
docker compose logs api

# Check port mappings
docker port taskforge-api
```

**Database connection failed:**
```bash
# Check DB container
docker compose logs db

# Test connection
docker compose exec db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Your_password123
```

---

## Common Problems

### Problem: Port 5000 Already in Use

```bash
# Solution 1: Stop other service
lsof -i :5000  # macOS/Linux
netstat -ano | findstr :5000  # Windows

# Solution 2: Change port in docker-compose.yml
# ports:
#   - "5001:8080"

# Solution 3: Kill process
kill -9 <PID>  # macOS/Linux
taskkill /PID <PID> /F  # Windows
```

### Problem: Docker Images Won't Build

```bash
# Solution: Clean and rebuild
docker compose down -v
docker system prune -a
docker compose up --build
```

### Problem: Database Migration Failure

```bash
# Solution 1: Reset database
dotnet ef database drop -p TaskForge.Infrastructure -s TaskForge.API
dotnet ef database update -p TaskForge.Infrastructure -s TaskForge.API

# Solution 2: View detailed error
dotnet ef database update --verbose -p TaskForge.Infrastructure -s TaskForge.API

# Solution 3: Check current migrations
dotnet ef migrations list -p TaskForge.Infrastructure
```

### Problem: NuGet Package Not Found

```bash
# Solution: Restore packages
dotnet restore

# Or clear NuGet cache
dotnet nuget locals all --clear
dotnet restore
```

### Problem: "Could not find a part of the path"

```bash
# Solution: Ensure you're in correct directory
pwd  # or cd in Windows
cd /path/to/taskforge

# Run the command
dotnet build
```

---

## Performance Tips

### Build Optimization

```bash
# Use incremental build
dotnet build

# Parallel compilation
dotnet build -m

# Release build (faster)
dotnet build -c Release
```

### Entity Framework Performance

```csharp
// ✅ Good - Single query
var users = await _context.Users
    .Include(u => u.Tasks)
    .ToListAsync();

// ❌ Bad - N+1 queries
var users = await _context.Users.ToListAsync();
foreach (var user in users)
{
    var tasks = await _context.Tasks
        .Where(t => t.UserId == user.Id)
        .ToListAsync();
}
```

### Local Development

```bash
# Use watch mode for faster feedback
dotnet watch build
dotnet watch test
dotnet watch run -p TaskForge.API

# Disable Docker when only testing code
dotnet test
```

---

## Resources

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

**Last Updated:** 2026-04-16

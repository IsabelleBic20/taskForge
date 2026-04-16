# TaskForge

TaskForge is a backend project built with .NET 8 using Clean Architecture principles. The goal is to provide a scalable and maintainable foundation for managing users, tasks, and workflows.

This project is designed as a professional portfolio application, including Docker, SQL Server, and Entity Framework Core.

---

## Architecture

The solution is organized into the following layers:

```
TaskForge/
├── TaskForge.API            # Presentation layer (HTTP endpoints)
├── TaskForge.Application    # Application logic and use cases
├── TaskForge.Domain         # Core entities and business rules
├── TaskForge.Infrastructure # Data access and external services
```

---

## Technologies

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server (Docker)
* Docker and Docker Compose
* Swagger (OpenAPI)

---

## Getting Started

### Prerequisites

* .NET SDK 8.0
* Docker
* Docker Compose

---

## Running the Project

Start the application and database using Docker:

```
docker compose up --build
```

---

## Application Access

* API: http://localhost:5000
* Swagger: http://localhost:5000/swagger

---

## Database

The project uses SQL Server running in a Docker container.

### Connection String (local development)

```
Server=localhost,1433;
Database=TaskForgeDb;
User Id=sa;
Password=Your_password123;
TrustServerCertificate=True;
```

---

## Migrations

### Create a migration

```
dotnet ef migrations add InitialCreate -p TaskForge.Infrastructure -s TaskForge.API
```

### Apply migrations

```
dotnet ef database update -p TaskForge.Infrastructure -s TaskForge.API
```

---

## Project Setup

1. Clone the repository
2. Run Docker
3. Apply database migrations
4. Access Swagger to test endpoints

---

## Features

* Dockerized environment
* SQL Server integration
* Entity Framework Core setup
* Initial database migration

Planned features:

* User management (CRUD)
* Authentication with JWT
* Task management
* Authorization and roles

---

## Project Goals

* Demonstrate Clean Architecture in .NET
* Provide a reusable backend template
* Showcase containerized development
* Serve as a professional portfolio project



## Author

Isabelle Bicudo
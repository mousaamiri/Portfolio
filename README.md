# MousaAmiri Portfolio

A multilingual portfolio website with admin panel, built with ASP.NET Core using Clean Architecture.

## Tech Stack

- **Backend:** .NET 10, ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **Testing:** xUnit + FluentAssertions + Moq
- **Architecture:** Clean Architecture (Monolith with internal layer separation)

## Project Structure

```
Portfolio/
├── src/
│   ├── Core/
│   │   ├── Portfolio.Domain/            # Entities, Value Objects, Enums
│   │   └── Portfolio.Application/       # Interfaces, DTOs, Use Cases/Services
│   ├── Infrastructure/
│   │   └── Portfolio.Infrastructure/    # EF Core, Repository implementations, Migrations
│   └── Presentation/
│       └── Portfolio.API/               # ASP.NET Core Web API (Public + Admin Controllers)
├── tests/
│   ├── Portfolio.Domain.Tests/
│   ├── Portfolio.Application.Tests/
│   ├── Portfolio.Infrastructure.Tests/
│   └── Portfolio.API.Tests/
├── .gitignore
├── Portfolio.sln
└── README.md
```

## Layers

### Domain (`Portfolio.Domain`)
The innermost layer containing business entities, value objects, and enums. Has no dependencies on other projects. Entity folders: `Projects`, `Skills`, `Experiences`, `Educations`.

### Application (`Portfolio.Application`)
Contains interfaces, DTOs, and service contracts. Depends only on Domain. Defines the application's use cases without knowing implementation details.

### Infrastructure (`Portfolio.Infrastructure`)
Implements the interfaces defined in Application. Contains EF Core DbContext, repository implementations, and migrations. Depends on Application and Domain.

### API (`Portfolio.API`)
The entry point of the application. Contains ASP.NET Core controllers split into `Public` (portfolio display) and `Admin` (content management) areas. Depends on Application (for service interfaces) and Infrastructure (for DI registration).

## Dependency Rule

```
Domain  <--  Application  <--  Infrastructure
                ^                    |
                |                    |
                +----  API  ---------+
```

Domain has zero dependencies. Each outer layer depends only on inner layers, never the reverse.

## Getting Started

```bash
# Build
dotnet build Portfolio.sln

# Run tests
dotnet test Portfolio.sln

# Run the API
dotnet run --project src/Presentation/Portfolio.API
```

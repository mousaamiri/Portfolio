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

### Prerequisites

- .NET 10 SDK
- SQL Server (or SQL Server LocalDB for development)

### 1. Clone and Build

```bash
dotnet build Portfolio.sln
```

### 2. Configure User Secrets

The JWT secret and admin password must be configured via User Secrets (never commit them to source control):

```bash
cd src/Presentation/Portfolio.API

# Initialize user secrets (if not done)
dotnet user-secrets init

# Set the JWT signing key (minimum 32 characters)
dotnet user-secrets set "JwtSettings:Secret" "YourSuperSecretKeyAtLeast32CharsLong!"

# Set the initial admin password
dotnet user-secrets set "AdminSeed:Password" "YourStrongAdminPassword123!"
```

### 3. Configure Database

Set your SQL Server connection string in user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=Portfolio;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 4. Run

```bash
dotnet run --project src/Presentation/Portfolio.API
```

On first startup, the `AdminSeeder` automatically creates the admin user if the `Admins` table is empty.

### 5. Run Tests

```bash
dotnet test Portfolio.sln
```

## Authentication

The API uses JWT Bearer authentication with a single admin user.

### Login

```
POST /api/admin/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "your-password"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-07-09T00:00:00Z"
}
```

### Using the Token

Include the JWT in the `Authorization` header for admin endpoints:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### Endpoint Authorization

| Path Pattern | Auth Required | Description |
|---|---|---|
| `api/public/*` | No | Public portfolio data |
| `api/admin/auth/login` | No | Login endpoint |
| `api/admin/*` | Yes (JWT) | Admin management endpoints |

### Rate Limiting

The login endpoint is rate-limited to 5 requests per minute per IP to prevent brute-force attacks.

### JWT Configuration (`appsettings.json`)

```json
{
  "JwtSettings": {
    "Secret": "",        // Set via User Secrets — never commit!
    "Issuer": "Portfolio.API",
    "Audience": "Portfolio.Client",
    "ExpiryInDays": 7
  }
}
```

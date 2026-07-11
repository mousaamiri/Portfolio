# MousaAmiri Portfolio

A bilingual (English / Persian) portfolio website with an admin panel, built with ASP.NET Core using Clean Architecture.

![Home page](docs/images/banner.png)

---

Portfolio is built for anyone who wants to showcase their work without touching code every time they finish a new project.
New projects and content get added and edited entirely through an admin panel, and show up on the live site instantly — no front-end changes, no redeploy.
This same dynamic behavior applies to every type of content on the site, so adding anything new always works the same simple way.

---

## Demo

[![Watch the demo](docs/images/demo-cover.png)](docs/demo.mp4)

> ▶️ Click the cover to play the walkthrough video (`docs/demo.mp4`).

## Screenshots

### Home

| English (amber) | Persian (RTL) |
|---|---|
| ![Home — English](docs/images/home-en.png) | ![Home — Persian](docs/images/home-fa.png) |

The whole site ships with a **live accent-color switcher** (the palette button, bottom-left) — every section re-themes instantly:

| English — emerald accent | Persian — indigo accent |
|---|---|
| ![Home — emerald accent](docs/images/home-en-emerald.png) | ![Home — indigo accent](docs/images/home-fa-indigo.png) |

### About & Contact

| About | Contact |
|---|---|
| ![About page](docs/images/about-en.png) | ![Contact page](docs/images/contact-en.png) |

## Features

- **Bilingual (EN / FA)** — language resolved server-side via a cookie, with full RTL support.
- **Live theming** — runtime accent-color and light/dark switching, no reload.
- **Admin panel** — JWT-secured CRUD over every content type (projects, skills, experience, articles, FAQs, …).
- **Dynamic content** — everything on the public site is database-backed and editable; no redeploy to publish.
- **Command palette** — `Cmd/Ctrl + K` for quick navigation.

## Tech Stack

- **Backend:** .NET 10, ASP.NET Core Web API
- **Frontend:** ASP.NET Core MVC (Razor Views) — consumes the API over HTTP
- **ORM:** Entity Framework Core (SQL Server)
- **Testing:** xUnit + FluentAssertions + Moq
- **Architecture:** Clean Architecture (monolith with internal layer separation)

## Project Structure

```
Portfolio/
├── src/
│   ├── Core/
│   │   ├── Portfolio.Domain/            # Entities, Value Objects, Enums
│   │   └── Portfolio.Application/       # Interfaces, DTOs, Use Cases/Services
│   ├── Infrastructure/
│   │   └── Portfolio.Infrastructure/    # EF Core, Repositories, Migrations, Seed
│   └── Presentation/
│       ├── Portfolio.API/               # ASP.NET Core Web API (Public + Admin Controllers)
│       └── Portfolio.Web/               # ASP.NET Core MVC front-end (consumes the API)
├── tests/
│   ├── Portfolio.Domain.Tests/
│   ├── Portfolio.Application.Tests/
│   ├── Portfolio.Infrastructure.Tests/
│   ├── Portfolio.API.Tests/
│   └── Portfolio.Web.Tests/
├── docs/                                # Screenshots and demo video
├── .gitignore
├── Portfolio.sln
└── README.md
```

## Layers

### Domain (`Portfolio.Domain`)
The innermost layer containing business entities, value objects, and enums. Has no dependencies on other projects.

### Application (`Portfolio.Application`)
Contains interfaces, DTOs, and service contracts. Depends only on Domain. Defines the application's use cases without knowing implementation details.

### Infrastructure (`Portfolio.Infrastructure`)
Implements the interfaces defined in Application. Contains the EF Core DbContext, repository implementations, migrations, and content seeding. Depends on Application and Domain.

### API (`Portfolio.API`)
The back-end entry point. Contains ASP.NET Core controllers split into `Public` (portfolio display) and `Admin` (content management) areas. Depends on Application (service interfaces) and Infrastructure (DI registration).

### Web (`Portfolio.Web`)
The MVC front-end. Renders the public site and the admin panel with Razor views, and talks to `Portfolio.API` over HTTP — it holds no direct database dependency.

## Dependency Rule

```
Domain  <--  Application  <--  Infrastructure
                ^                    |
                |                    |
                +----  API  ---------+
                        ^
                        |  (HTTP)
                      Web
```

Domain has zero dependencies. Each outer layer depends only on inner layers, never the reverse. The Web front-end depends on the API only over HTTP.

## Getting Started

### Prerequisites

- .NET 10 SDK (see `global.json` — `10.0.301`)
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

The site is two processes — start the API first, then the Web front-end:

```bash
# Terminal 1 — API
dotnet run --project src/Presentation/Portfolio.API

# Terminal 2 — Web front-end
dotnet run --project src/Presentation/Portfolio.Web
```

On first startup the `AdminSeeder` creates the admin user (if the `Admins` table is empty) and the `ContentSeeder` seeds the real portfolio content. The Web front-end reaches the API via the `PortfolioApi:BaseUrl` setting in its `appsettings.json`.

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

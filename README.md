# Legal API System

## Overview
The Legal API System is a modular .NET 9 web application built with Clean Architecture & DDD. It provides a foundation for legal document management and multi-domain business modules (current: Admin; future: Shop, Chat).

## ⚙️ Architecture
Layers:
- API (`Legal.Api.WebApi`) – HTTP endpoints, Swagger, auth pipeline
- Application (`Legal.Application.Admin`) – CQRS handlers, business rules, validation
- Service (`Legal.Service.*`) – Infrastructure concerns (EF, Repos, external services, helpers)
- Shared (`Legal.Shared.SharedModel`) – Cross-cutting models / DTOs / abstractions
- Migration Service (`MigrationService`) – Multi–DbContext migration & bootstrapping
- Frontend (`Legal.Website.esproj`) – SPA (Angular) website project integrated via Visual Studio JavaScript tooling

## 🚀 Features
- Modular, pluggable module naming convention
- CQRS (segregated Command & Query endpoints with dynamic handler discovery)
- JWT auth & role-based authorization
- File upload (multipart) + file download pipeline
- PostgreSQL via EF Core (snake_case naming)
- Background data seeding (JSON driven, upsert style with optional update)
- Dockerized development & deployment workflow (CLI & Visual Studio)
- Centralized request execution abstraction
- Frontend SPA served via dedicated container / static host

## 📁 Project Structure
```
Legal.Api.Solution/
├── Api/Legal.Api.WebApi/
├── Application/Legal.Application.Admin/
├── Service/
│   ├── Legal.Service.Infrastructure/
│   ├── Legal.Service.Repository/
│   └── Legal.Service.Helper/
├── Shared/Legal.Shared.SharedModel/
├── Website/Legal.Website/                # Frontend (Legal.Website.esproj)
└── OtherServices/MigrationService/
```

## 🛠️ Technologies
.NET 9, ASP.NET Core, EF Core (Npgsql), AutoMapper, FluentValidation, Newtonsoft.Json, SignalR, Swagger/OpenAPI, Docker, ImageSharp, FFMpegCore, Angular (Website), Nginx (container serve).

## 📋 Prerequisites
- .NET 9 SDK
- PostgreSQL 12+
- (Optional) Docker / Docker Compose
- VS 2022 / VS Code (VS recommended for integrated Docker + JS project)

## ⚡ Quick Start (Local CLI)
### 1. Clone
```bash
git clone <repository-url>
cd Legal.Api.Solution
```
### 2. Configure Connection String
Use either appsettings.Development.json, user-secrets, or environment variables:
```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Database=LegalDB;Username=postgres;Password=password;Port=5432"
```
### 3. Run Migrations (Option A – EF CLI)
```bash
dotnet ef database update --project Application/Legal.Application.Admin
```
### 3. Run Migrations (Option B – MigrationService)
```bash
dotnet run --project OtherServices/MigrationService -- --update-database "y" --context-number "-1"
```
### 4. (Optional) Seed Data
Add InitializationOptions to Api appsettings (see Data Seeding section) and run the API once.
### 5. Start API
```bash
dotnet run --project Api/Legal.Api.WebApi
```
### 6. Open Swagger
https://localhost:5001/swagger

## 🖥️ Run with Docker in Visual Studio
The solution is configured for Visual Studio container tools using Docker Compose (see docker-compose.dcproj).

Steps:
1. Install VS 2022 workloads: 
   - ASP.NET and web development
   - .NET + Container Development Tools
   - JavaScript/TypeScript (for Website project)
2. Open solution (.sln). Visual Studio detects Docker support from:
   - Api/Legal.Api.WebApi.csproj (DockerDefaultTargetOS, DockerfileContext)
   - Website/Legal.Website/Legal.Website.esproj (JavaScript project with Docker metadata)
   - docker-compose.dcproj at root.
3. Set Startup Project: choose the docker-compose project in Solution Explorer (right‑click Set as Startup). This launches all defined containers (API, Website, MigrationService, PostgreSQL, etc.).
4. Configure Environment Variables: edit docker-compose.override.yml (or compose file) for:
   - ConnectionStrings__Postgres
   - ASPNETCORE_ENVIRONMENT=Development
5. Press F5:
   - Visual Studio builds container images (Fast mode if enabled) and starts containers.
   - MigrationService runs first (if included) to apply migrations.
6. Access:
   - Frontend Website container (e.g., http://localhost:8080)
   - API Swagger (e.g., http://localhost:4020/swagger)
7. Debugging:
   - API: Breakpoints work normally in .cs code.
   - Website: Use browser dev tools; for live Angular development you can also run npm start outside container (Website project has StartupCommand npm start) and only keep API in Docker.
8. Logs: Use View > Other Windows > Containers window in VS or run `docker compose logs -f` externally.
9. Hot Reload: .NET Hot Reload applies to the API when using Fast (mounted) mode; for Angular use its dev server outside Docker during active UI work.

If docker-compose is not automatically added:
- Right-click solution > Add > Container Orchestrator Support > Docker Compose.
- Add existing Api & Website projects to docker-compose.yml services.

## 🧩 Modules
Current:
- Admin: Users, auth, roles, system management
Planned:
- Shop: Catalog, products, pricing
- Chat: Realtime messaging, presence

## 🔒 Security
- JWT bearer tokens
- Role-based policy authorization
- CORS configured via AllowedOrigins
- FluentValidation input enforcement

## 🗄️ Database & Migrations
- Provider: PostgreSQL (Npgsql)
- Style: Code-first EF Core
- Migration execution:
  - EF CLI (module specific)
  - Central MigrationService (multi-context, discovery, command line flags)

### MigrationService Usage
```bash
# All contexts (auto apply)
dotnet run --project OtherServices/MigrationService -- --update-database "y" --context-number "-1"

# Specific contexts (comma or range)
--context-number "1"     # single
--context-number "1,3"   # multiple
--context-number "1-4"   # range
```
Optional overrides:
```
--data-db "Host=..."  # override primary DB
```

## 🌱 Data Seeding
A background service in the API reads InitializationOptions:
```json
"InitializationOptions": [
  { "FilePath": "Seed/admin_users.json", "DoUpdate": true },
  { "FilePath": "Seed/roles.json", "DoUpdate": false }
]
```
Each JSON file: array of objects. Records with new id are inserted; if DoUpdate=true existing ones are updated (excluding id). Timestamps (last_modified_time) are auto-updated.
Place seed JSON files under a path copied to output (e.g., Api/Legal.Api.WebApi/Seed/...).

## 📡 API Endpoints (Summary)
Command:
- POST /api/Command/Execute/{module}
- POST /api/Command/Upload/{module}
- GET  /api/Command/ListAll/{module}
- GET  /api/Command/Detail/{module}/{name}
Query:
- POST /api/Query/Execute/{module}
- GET  /api/Query/ListAll/{module}
- GET  /api/Query/Detail/{module}/{name}
- POST /api/Query/download/{module}
Public: Module-specific unauthenticated endpoints.

## ⚙️ Configuration
Environment Variables / User Secrets keys:
- ConnectionStrings:Postgres
- AllowedOrigins (CSV)
- MaxRequestBodySize (bytes)

Environment variable style for containers:
- ConnectionStrings__Postgres
- AllowedOrigins

## 🐳 Docker (CLI)
```bash
docker compose up -d --build
```
Adjust env vars in compose file as needed. For Visual Studio workflow see earlier section.

## 🧪 Testing
Planned additions:
- xUnit test projects per layer
- Testcontainers for PostgreSQL integration tests
- FluentAssertions for expressive assertions
- Minimal API contract tests via Microsoft.AspNetCore.Mvc.Testing

## 📝 License
Specify a license (e.g., MIT). Example:
MIT License © Taufiq Abdur Rahman
You may not use this codebase without permission.

## 💬 Support
Open an issue with reproduction steps & environment details.

## 📚 Additional Docs
- Api/Legal.Api.WebApi/README.md
- Application/Legal.Application.Admin/README.md
- Service/Legal.Service.Infrastructure/README.md
- Service/Legal.Service.Repository/README.md
- Service/Legal.Service.Helper/README.md
- Shared/Legal.Shared.SharedModel/README.md
- OtherServices/MigrationService/README.md
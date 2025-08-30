# Legal API System

## Overview
The Legal API System is a modular monolithic .NET 9 web application built with Clean Architecture & DDD. It provides a foundation for legal document management and multi-domain business modules (current: Admin).

## 🧬 Modular Monolithic Architecture (Key Traits)
This codebase follows a Modular Monolith pattern: one deployable unit, multiple strongly isolated business modules.

Core characteristics:
- Explicit Module Enumeration: `ApplicationHelper.ModuleName` enum defines all module identities (e.g. ADMIN, future SHOP, CHAT) for routing, seeding, authorization scopes.
- Vertical Slice Design: Each module owns its Commands, Queries, Parameter / Response models, validators, mappings, DbContext (current Admin). Cross-module chatter only through well-defined Shared contracts.
- Dynamic Handler Discovery: At startup assemblies are scanned; classes inheriting generic base types (e.g. ACommandHandler<,>, AQueryHandler<,>) are auto-registered. No manual endpoint wiring.
- Unified API Surface: Single WebApi host exposes per-module dynamic endpoints plus generic execution controllers. Module name is part of the URL path ensuring clarity and separation.
- Per-Module Data Concerns: Each module can introduce its own EF Core DbContext + migrations. MigrationService can run all or selected contexts via CLI switches (range / list / all).
- Shared Kernel Library: `Legal.Shared.SharedModel` supplies only cross-cutting primitives (DTOs, parameter models, validators) avoiding bidirectional dependencies between feature modules.
- Encapsulated Infrastructure: Common infra (repositories, helpers, storage, directory utilities) lives under Service layer; modules consume via interfaces, enabling future extraction to microservices if needed.
- Consistent Naming & Discovery: Naming conventions (e.g. *ParameterModel, *ResponseModel, *Handler) allow reflection-based registration & endpoint generation.
- Feature Growth Path: New module can be added without altering existing module internals; only enum extension + DI scan + (optional) new migrations.
- Single Deployment Unit: Operational simplicity (one process/image) while preserving logical boundaries.
- Evolution Friendly: Clear seams enable later decomposition to services if scaling / independent deployment requirements emerge.

### Adding a New Module (Example: SHOP)
1. Extend Module Enum: Add `SHOP` to `ApplicationHelper.ModuleName`.
2. Create Application Project (if separate) or folder structure mirroring Admin (Commands, Queries, Validators, DbContext, Migrations).
3. Implement DbContext (e.g. `ShopDbContext`) and add EF configuration & migrations.
4. Add Command / Query handlers inheriting base handler abstractions; include corresponding Parameter / Response models (or reuse Shared ones).
5. Register Assembly: Ensure new project referenced by WebApi; existing reflection registration picks up handlers automatically.
6. Seed Data (optional): Provide JSON seed file path in API `InitializationOptions` for module bootstrap.
7. Run MigrationService: Include new context (auto-discovered) or pass specific `--context-number`.
8. Call Endpoints: Use dynamic endpoints `/api/{module}/commands/{Name}` and `/api/{module}/queries/{Name}` where `{module}` = `shop` (case-insensitive mapping to enum).

### Benefits Over Classic Monolith
- Reduced Coupling: Module boundaries enforced via separate folders / projects + limited shared surface.
- Faster Refactoring: Adding features confined to module vertical slice.
- Performance: In-process communication (no network penalty) while still structured.
- Operational Simplicity: Single build, single docker image, unified logging & tracing.
- Gradual Service Extraction: Any module can later be extracted with minimal churn due to isolation & explicit contracts.

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
├── Website/Legal.Website/
└── OtherServices/MigrationService/
```

## 🛠️ Technologies
.NET 9, ASP.NET Core, EF Core (Npgsql), AutoMapper, FluentValidation, Newtonsoft.Json, SignalR, Swagger/OpenAPI, Docker, ImageSharp, FFMpegCore, Angular (Website), Nginx (container serve).

## 📋 Prerequisites
- .NET 9 SDK
- PostgreSQL 12+
- (Optional Recommended) Docker / Docker Compose
- VS 2022 / VS Code (VS recommended for integrated Docker + JS project)

## 🖥️ Run with Docker in Visual Studio (Recommended)
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
   - Frontend Website container (http://localhost:8080)
   - API Swagger (http://localhost:4020/swagger)
7. Debugging:
   - API: Breakpoints work normally in .cs code.
   - Website: Use browser dev tools; for live Angular development you can also run npm start outside container (Website project has StartupCommand npm start) and only keep API in Docker.
8. Logs: Use View > Other Windows > Containers window in VS or run `docker compose logs -f` externally.
9. Hot Reload: .NET Hot Reload applies to the API when using Fast (mounted) mode; for Angular use its dev server outside Docker during active UI work.

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

## 🧩 Modules
Current:
- Admin: Users, Auth, Contract

Planned:
- Shop, Chat (scaffold-ready via enum expansion and handler assembly)

## 🔒 Security
- JWT bearer tokens
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
Current test coverage focuses on the Admin Application layer.

Project:
- Tests/Legal.Application.Admin.Tests (xUnit, FluentAssertions, Moq, EFCore.InMemory, Coverlet)

Run all tests:
```bash
dotnet test
```

Collect code coverage (lcov for report tools like ReportGenerator or Sonar):
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutput=../TestResults/ /p:CoverletOutputFormat=lcov
```

Naming convention:
`MethodUnderTest_Scenario_ExpectedResult`

Examples (see source):
- RegistrationServiceTests.RegisterUserAsync_NewUser_PersistsAndReturnsDto
- RegistrationServiceTests.RegisterUserAsync_UserAlreadyExists_Throws
- ContractServiceTests.Get_ReturnsMappedDto_WhenFound

Testing patterns employed:
- Arrange/Act/Assert with clear separation
- Moq strict mocks to ensure only expected repository calls are made
- FluentAssertions for expressive assertions
- AutoMapper configured ad‑hoc per test class to keep mapper surface minimal
- Password hashing verified using helper (no direct implementation coupling)

Writing a new service/handler test:
1. Configure minimal AutoMapper profile(s) for DTO <-> Entity
2. Mock repository (IRepository / IDomainRepository) methods used by the handler/service
3. Instantiate target service/handler directly (no full DI container required)
4. Execute method under test with CancellationToken.None (or a token you can cancel for cancellation tests)
5. Assert returned DTO shape & side effects (Verify mocks, verify password hashing, etc.)
6. (Optional) Add negative path tests (validation failure, existing entity, not found, etc.)

Mapping validation (optional guard test):
```csharp
var config = new MapperConfiguration(cfg => cfg.AddProfile<MyProfile>());
config.AssertConfigurationIsValid();
```

Using EFCore.InMemory (for behaviour that depends on LINQ translation):
- Prefer to still mock repository unless you explicitly need query translation semantics.
- If used, seed context, execute, assert results.

Planned future test additions:
- Testcontainers PostgreSQL integration tests (real migrations + data seeding)
- Minimal API endpoint contract tests (Microsoft.AspNetCore.Mvc.Testing)
- Authentication / authorization pipeline tests
- Performance benchmarks for heavy queries

Quick watch mode (reruns on change) using dotnet-watch:
```bash
dotnet watch --project Tests/Legal.Application.Admin.Tests test
```

## ⚠️ Disclaimer
- Backend architecture was originally designed and written by the author for a personal project and reused here (no AI assistance for backend implementation).
- All documentation content was generated with GitHub Copilot and reviewed/edited by the author.
- Frontend boilerplate was generated using GitHub Copilot and then manually updated/refined.
- Used AI to
	- fixed Issue for dockization 
	- frontend unit tests

## 📝 License
MIT License © Taufiq Abdur Rahman
You may not use this codebase without permission. For Evolution purposes only.

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
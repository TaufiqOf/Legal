# Legal Contract Management System - Complete Setup Guide

## Overview
End‑to‑end contract management platform consisting of:
- .NET 9 Web API (CQRS + modular Admin domain)
- Angular 20 SPA frontend (Legal.Website)
- PostgreSQL database (EF Core code‑first)
- JWT authentication & role authorization
- Optional background data seeding + dedicated MigrationService

## System Architecture
```
┌──────────────────┐     HTTPS/REST     ┌──────────────────┐     ADO / EF Core     ┌───────────────┐
│  Angular SPA      │  <-------------->  │  .NET 9 API       │  <------------------->│  PostgreSQL    │
│  (Legal.Website)  │  SignalR (future) │  (Legal.Api.WebApi)│                     │  (legaldb)     │
└──────────────────┘                    │  CQRS Handlers    │                     └───────────────┘
                                        │  Auth / Validation│
                                        │  Background Seed  │
                                        └──────────────────┘
```
Ancillary:
- MigrationService (applies EF migrations across contexts before API start when containerized)

## Repository Layout (Relevant)
```
Api/Legal.Api.WebApi/
Application/Legal.Application.Admin/
Shared/Legal.Shared.SharedModel/
Service/Legal.Service.*
Website/Legal.Website/               (Angular project, Legal.Website.esproj)
OtherServices/MigrationService/
```

## Backend Components
### API: Api/Legal.Api.WebApi
- Command & Query endpoints (dynamic handler execution)
- Swagger/OpenAPI UI
- JWT Auth, CORS, Upload/Download
- Background JSON seeding service

### Application Layer: Application/Legal.Application.Admin
- CQRS command/query handlers
- Admin EF DbContext
- Validation (FluentValidation) & mapping

### Shared: Shared/Legal.Shared.SharedModel
- DTOs, SQL helper models, abstractions

### Migration Service: OtherServices/MigrationService
- Console utility to discover contexts & run migrations
- CLI switches: `--update-database "y|n"`, `--context-number "-1|list|range"`, `--data-db "..."`

## Frontend Component
### Angular SPA: Website/Legal.Website
- Managed by Legal.Website.esproj (VS JavaScript project system)
- Uses `npm start` (dev server) & Docker (nginx) for production

Correct path note: Earlier docs referenced `Legal.Website/` at root; actual path is `Website/Legal.Website/`.

## Ports (Typical Defaults)
- API (dev with Kestrel / HTTPS): 5001 (HTTPS) / 5000 (HTTP) OR 4020 when using compose (see docker files)
- Website (Angular dev server): 4200
- Website (nginx container): 8080
- PostgreSQL: 5432
(Adjust according to docker-compose or launchSettings.json.)

## Environment Configuration
Backend (env vars / user secrets):
- `ConnectionStrings:Postgres` or `ConnectionStrings__Postgres`
- `AllowedOrigins`
- `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` (if implemented)
Frontend build‐time (environment.ts / prod):
- `apiBaseUrl`

## Setup Paths
| Task | Path |
|------|------|
| API Project | Api/Legal.Api.WebApi |
| Angular Project | Website/Legal.Website |
| Migration Service | OtherServices/MigrationService |
| Seed JSON | Api/Legal.Api.WebApi/Seed (example) |

---
## Local Development (Manual / CLI)
### 1. Database
Ensure PostgreSQL running and create database (if not auto created):
```sql
CREATE DATABASE "LegalDB";
```
### 2. Configure API Connection String
```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Database=LegalDB;Username=postgres;Password=postgres;Port=5432" --project Api/Legal.Api.WebApi
```
### 3. Apply Migrations
Option A (direct EF):
```bash
dotnet ef database update --project Application/Legal.Application.Admin
```
Option B (central service):
```bash
dotnet run --project OtherServices/MigrationService -- --update-database "y" --context-number "-1"
```
### 4. (Optional) Data Seeding
Add to Api/Legal.Api.WebApi appsettings.Development.json:
```json
"InitializationOptions": [
  { "FilePath": "Seed/admin_users.json", "DoUpdate": true }
]
```
Ensure JSON file is copied to output (`Copy to Output Directory`). Run API once.
### 5. Run API
```bash
dotnet run --project Api/Legal.Api.WebApi
```
Swagger: https://localhost:5001/swagger
### 6. Frontend
```bash
cd Website/Legal.Website
npm install
npm start
```
Browser: http://localhost:4200

---
## Visual Studio Workflow (Docker Compose)
1. Install workloads: ASP.NET + Containers + JS/TS.
2. Open solution; ensure `docker-compose` project present.
3. Set docker-compose as Startup (contains API, Website, MigrationService, PostgreSQL services).
4. Optional: Edit `docker-compose.override.yml` for port & env overrides (e.g., API_PORT, WEB_PORT).
5. Press F5 → VS builds images, runs MigrationService (migrations), then API & Website.
6. Access:
   - Website (nginx): http://localhost:8080
   - API: http://localhost:4020/swagger
7. Debugging: Breakpoints in API; for Angular hot reload prefer `npm start` outside Docker while API stays containerized.

---
## Docker CLI Workflow (Without VS)
```bash
# Build & start all services
docker compose up -d --build

# View logs
docker compose logs -f

# Rebuild specific service
docker compose build legal.api.webapi
```
Stop / Remove:
```bash
docker compose down        # keep volumes
docker compose down -v     # remove volumes/data
```

### Overriding Environment
Create `.env` (if not already):
```
POSTGRES_DB=legaldb
POSTGRES_USER=legaluser
POSTGRES_PASSWORD=legalpass
API_PORT=4020
WEB_PORT=8080
```

---
## API Interaction (CQRS Endpoints)
All commands/queries POST JSON payloads containing handler name & data (pattern representative):
```
POST /api/Command/Execute/ADMIN
POST /api/Query/Execute/ADMIN
```
Example Login (command body shape may vary):
```json
{
  "name": "LogInCommandHandler",
  "data": { "userName": "admin", "password": "P@ssw0rd" }
}
```
Paged Contracts Query:
```json
{
  "name": "GetByPagedContractQueryHandlers",
  "data": { "page": 1, "pageSize": 20, "search": "" }
}
```
Save Contract Command:
```json
{
  "name": "SaveContractCommandHandler",
  "data": { "id": null, "name": "NDA", "author": "Admin", "description": "Mutual NDA" }
}
```
Delete Contract Command:
```json
{
  "name": "DeleteContractCommandHandler",
  "data": { "id": "<contract-id>" }
}
```

### Contract Model (Example)
```typescript
interface Contract {
  id: string;
  name: string;
  author: string;
  description: string;
  created: string; // ISO date
  updated?: string;
}
```

---
## User Workflow Summary
1. Register / login via command endpoint, obtain JWT.
2. Store token in frontend (localStorage / memory). Attach Authorization header.
3. Browse paginated contracts, create/update/delete.
4. Logout clears token.

---
## Security
- JWT authorization header: `Authorization: Bearer <token>`
- CORS restricted by AllowedOrigins
- Password hashing (backend) + validation
- Planned: refresh tokens, role expansion

---
## Development Productivity
- Hot Reload (API) via `dotnet watch` (optional)
- Angular dev server live reload
- Centralized exception handling + logging

---
## Production Deployment Checklist
Backend:
- Set `ASPNETCORE_ENVIRONMENT=Production`
- Provide strong JWT key (user secrets / env var)
- Run MigrationService (one-off) or ensure automatic migration job
- Reverse proxy / HTTPS termination (nginx / Azure / AWS ALB)
Frontend:
```bash
npm run build
# Deploy dist/Legal.Website to static host or container image build
```
Database:
- Provision managed PostgreSQL / secure network
- Backups & monitoring

---
## Troubleshooting
| Symptom | Check |
|---------|-------|
| 404 on SPA refresh | nginx/SPA rewrite config (in Docker image) |
| CORS error | AllowedOrigins value & protocol/port accuracy |
| Auth fails | JWT key/issuer mismatch, clock skew |
| DB connect fail | Connection string / container network / port mapping |
| Migration hang | Lock or failed previous migration; inspect MigrationService logs |

Logs:
```bash
docker compose logs -f legal.api.webapi
docker compose logs -f legal.website
```

---
## Support
1. Review root README & DOCKER_DEPLOYMENT.md
2. Check container & application logs
3. Verify environment variables & connection strings

---
(End of Guide)
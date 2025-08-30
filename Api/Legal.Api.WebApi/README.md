# Legal.Api.WebApi

## Overview

The main Web API project that serves as the entry point for the Legal API System. This ASP.NET Core Web API provides RESTful endpoints for command execution, query processing, and file operations across multiple business modules.

## 🏗️ Architecture

This API follows a CQRS style with dynamic handler discovery. Handlers (command & query) are registered per module and can be executed in three ways:

1. Aggregated generic endpoints (CommandController / QueryController) that accept a RequestName + Parameter payload.
2. Public (anonymous) execution endpoints for explicitly allowed handlers.

### Key Components

- **Controllers** - Conventional API endpoint controllers
- **Program.cs** - Application startup and configuration (maps dynamic endpoints)
- **Data Seeding** - Initial data population
- **Delegate Handlers** - Request/response pipeline handlers

## 📂 Project Structure

```
Legal.Api.WebApi/
├── Controllers/
│   ├── CommandController.cs        # Generic command execution endpoints
│   ├── QueryController.cs          # Generic query execution endpoints
│   └── PublicController.cs         # Anonymous/public endpoints
├── DynamicHandlerEndpoints.cs      # Creates per-handler endpoints
├── DataSeeding/
│   ├── DataSeederService.cs        # Background data seeding service
│   └── InitializationOptions.cs    # Seeding configuration
├── DelegateHandler/
│   └── TokenDelegateHandler.cs     # JWT token handling
├── Properties/
│   └── launchSettings.json         # Development launch settings
├── Program.cs                      # Application entry point
└── README.md                       # This file
```

## ✨ Features

### Command Processing
- Execute business commands with validation
- File upload support with multipart form data
- Command discovery and documentation
- Dynamic per-command endpoints
- Error handling and logging

### Query Processing
- Execute read operations and data retrieval
- File download capabilities
- Query discovery and documentation
- Dynamic per-query endpoints
- Flexible result handling

### Security
- JWT token authentication
- Role-based authorization
- CORS configuration
- Request size limitations

### Real-time Features
- SignalR integration (prepared)
- Redis backplane support
- MessagePack protocol support

## 🔌 API Endpoints

### 1. Dynamic Per-Handler Endpoints (Minimal APIs)
Automatically generated for each registered handler using the pattern:

```
POST /api/{module}/commands/{CommandName}
POST /api/{module}/queries/{QueryName}
```

Example (ADMIN module):
```http
POST /api/admin/commands/SaveContract
Content-Type: application/json

{
  "Title": "Agreement A",
  "EffectiveDate": "2025-09-01T00:00:00Z"
}
```
```http
POST /api/admin/queries/GetContract
Content-Type: application/json

{
  "Id": "b0a4c9d2-3b6c-4d2f-b6e3-9c2a7f1e1c11"
}
```
Payloads map directly to the handler parameter model (no wrapper object required). Internally the API wraps this into the generic execution contract.

### 2. Generic Command Controller (`/api/Command`)

#### Execute Command
```http
POST /api/Command/Execute/{moduleName}
Content-Type: application/json
{
  "RequestName": "CommandName",
  "Parameter": { /* command params */ }
}
```

#### Upload Files
```http
POST /api/Command/Upload/{moduleName}
Content-Type: multipart/form-data

data: {"RequestName": "UploadCommand", "Parameter": {...}}
files: [file1, file2, ...]
```

#### List Commands
```http
GET /api/Command/ListAll/{moduleName}
```

#### Get Command Details
```http
GET /api/Command/Detail/{moduleName}/{commandName}
```

### 3. Generic Query Controller (`/api/Query`)

#### Execute Query
```http
POST /api/Query/Execute/{moduleName}
Content-Type: application/json
{
  "RequestName": "QueryName",
  "Parameter": { /* query params */ }
}
```

#### Download Files
```http
POST /api/Query/download/{moduleName}
Content-Type: application/json
{
  "RequestName": "DownloadQuery",
  "Parameter": { /* download params */ }
}
```

#### Get File by ID
```http
GET /api/Query/{moduleName}/{requestName}/file/{id}
```

#### List Queries
```http
GET /api/Query/ListAll/{moduleName}
```

#### Get Query Details
```http
GET /api/Query/Detail/{moduleName}/{queryName}
```

### 4. Public (Anonymous) Endpoints (`/api/public`)
Used for handlers explicitly marked `[AllowAnonymous]`.

```http
POST /api/public/execute/{moduleName}
```

### Selecting Execution Style
| Scenario | Recommended Endpoint Type |
|----------|---------------------------|
| Simple client wants strongly typed REST style | Dynamic per-handler endpoint |
| Needs reflection / discovery | Generic ListAll + Detail endpoints |
| Mixed file upload + command | `/api/Command/Upload/{module}` |
| Anonymous access | `/api/public/execute/{module}` |

## ⚙️ Configuration

### Application Settings

```json
{
  "ConnectionStrings": { "Postgres": "connection-string" },
  "AllowedOrigins": ["http://localhost:3000"],
  "MaxRequestBodySize": "100000000",
  "Logging": { "LogLevel": { "Default": "Information" } }
}
```

### User Secrets

Sensitive configuration is stored in User Secrets:
- Database connection strings
- API keys
- JWT secrets

### Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `ASPNETCORE_URLS` - Listening URLs

## 📦 Dependencies

### NuGet Packages

- Microsoft.AspNetCore.Mvc.NewtonsoftJson
- Microsoft.AspNetCore.SignalR.*
- Microsoft.EntityFrameworkCore.Design
- Swashbuckle.AspNetCore
- Microsoft.VisualStudio.Azure.Containers.Tools.Targets

### Project References

- Legal.Application.Admin
- Legal.Service.Infrastructure
- Legal.Service.Repository
- Legal.Shared.SharedModel

## 🚀 Running the Application

### Development Mode

```bash
dotnet run
```

### Production Mode

```bash
dotnet build --configuration Release
dotnet run --configuration Release
```

### Docker
```bash
docker build -t legal-api .
docker run -p 8080:8080 legal-api
```

## 📘 Swagger Documentation

Development URLs:
- https://localhost:5001/swagger
- http://localhost:5000/swagger

Swagger includes:
- Generic endpoints
- Dynamic per-handler endpoints
- Handler list injected via document filter

## 🔐 Authentication

Authorization header:
```http
Authorization: Bearer <jwt-token>
```

## 🪵 Logging
Structured logging (console + extensible sinks).

## ❗ Error Handling

Typical error response:
```json
{ "Success": false, "Error": "Error description" }
```

## 🌐 CORS Configuration
Flexible policy with allow-list or permissive dev mode.

## 🧩 Module System
Modules (ADMIN, SHOP, CHAT) register their handlers. Each module gets its own endpoint namespace.

## 🧪 Testing
Use Swagger UI or Postman. (Integration tests TBD.)

## 🚢 Deployment
See Docker section above / compose file.

## 💻 Development
Add new handler in a module project; it is auto-registered and a dynamic endpoint appears (no controller changes required).

## 📚 References
- ASP.NET Core Docs
- EF Core Docs
- SignalR Docs
- Swagger/OpenAPI Docs

---
## License
MIT License © Taufiq Abdur Rahman
You may not use this codebase without permission. For Evolution purposes only.

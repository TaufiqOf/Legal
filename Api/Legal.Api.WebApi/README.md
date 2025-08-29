# Legal.Api.WebApi

## Overview

The main Web API project that serves as the entry point for the Legal API System. This ASP.NET Core Web API provides RESTful endpoints for command execution, query processing, and file operations across multiple business modules.

## ??? Architecture

This API follows the mediator pattern with a centralized request handler that routes commands and queries to appropriate handlers based on module names.

### Key Components

- **Controllers** - API endpoint controllers
- **Program.cs** - Application startup and configuration
- **Data Seeding** - Initial data population
- **Delegate Handlers** - Request/response pipeline handlers

## ?? Project Structure

```
Legal.Api.WebApi/
??? Controllers/
?   ??? CommandController.cs       # Command execution endpoints
?   ??? QueryController.cs         # Query execution endpoints
?   ??? PublicController.cs        # Public access endpoints
??? DataSeeding/
?   ??? DataSeederService.cs       # Background data seeding service
?   ??? InitializationOptions.cs   # Seeding configuration
??? DelegateHandler/
?   ??? TokenDelegateHandler.cs    # JWT token handling
??? Properties/
?   ??? launchSettings.json        # Development launch settings
??? Program.cs                     # Application entry point
??? README.md                      # This file
```

## ?? Features

### Command Processing
- Execute business commands with validation
- File upload support with multipart form data
- Command discovery and documentation
- Error handling and logging

### Query Processing
- Execute read operations and data retrieval
- File download capabilities
- Query discovery and documentation
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

## ?? API Endpoints

### Command Controller (`/api/Command`)

#### Execute Command
```http
POST /api/Command/Execute/{moduleName}
Content-Type: application/json

{
  "RequestName": "CommandName",
  "Parameter": {
    // Command-specific parameters
  }
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

### Query Controller (`/api/Query`)

#### Execute Query
```http
POST /api/Query/Execute/{moduleName}
Content-Type: application/json

{
  "RequestName": "QueryName",
  "Parameter": {
    // Query-specific parameters
  }
}
```

#### Download Files
```http
POST /api/Query/download/{moduleName}
Content-Type: application/json

{
  "RequestName": "DownloadQuery",
  "Parameter": {
    // Download parameters
  }
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

## ?? Configuration

### Application Settings

```json
{
  "ConnectionStrings": {
    "Postgres": "connection-string"
  },
  "AllowedOrigins": ["http://localhost:3000"],
  "MaxRequestBodySize": "100000000",
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
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

## ?? Dependencies

### NuGet Packages

- **Microsoft.AspNetCore.Mvc.NewtonsoftJson** - JSON serialization
- **Microsoft.AspNetCore.SignalR.*** - Real-time communication
- **Microsoft.EntityFrameworkCore.Design** - EF Core design-time tools
- **Swashbuckle.AspNetCore** - API documentation
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** - Docker support

### Project References

- **Legal.Application.Admin** - Admin module business logic
- **Legal.Service.Infrastructure** - Core infrastructure services
- **Legal.Service.Repository** - Data access layer
- **Legal.Shared.SharedModel** - Shared models and DTOs

## ????? Running the Application

### Development Mode

```bash
# Using .NET CLI
dotnet run

# Using Visual Studio
F5 or Ctrl+F5
```

### Production Mode

```bash
# Build and run
dotnet build --configuration Release
dotnet run --configuration Release

# Using Docker
docker build -t legal-api .
docker run -p 8080:8080 legal-api
```

## ?? Swagger Documentation

When running in development mode, Swagger UI is available at:
- `https://localhost:5001/swagger`
- `http://localhost:5000/swagger`

The Swagger documentation provides:
- Interactive API testing
- Request/response schemas
- Authentication testing
- Example requests

## ?? Authentication

### JWT Token Flow

1. **Login** - Send credentials to authentication endpoint
2. **Token** - Receive JWT token in response
3. **Authorization** - Include token in Authorization header
4. **Validation** - Token is validated on each request

### Authorization Header

```http
Authorization: Bearer <jwt-token>
```

## ?? Logging

The application uses structured logging with:
- **Console** - Development logging
- **File** - Production logging (when configured)
- **Exception details** - Detailed error information

### Log Levels

- **Error** - Exception handling and critical errors
- **Warning** - Non-critical issues
- **Information** - General application flow
- **Debug** - Detailed debugging information

## ?? Error Handling

### Response Format

```json
{
  "Success": false,
  "Message": "Error description",
  "Data": null,
  "Errors": ["Detailed error messages"]
}
```

### HTTP Status Codes

- **200 OK** - Successful operation
- **400 Bad Request** - Invalid request or validation errors
- **401 Unauthorized** - Authentication required
- **403 Forbidden** - Insufficient permissions
- **404 Not Found** - Resource not found
- **500 Internal Server Error** - Server errors

## ?? CORS Configuration

The API supports flexible CORS configuration:
- **Development** - Allow all origins with credentials
- **Production** - Configured allowed origins only
- **Headers** - Allow all headers
- **Methods** - Allow all HTTP methods

## ?? Module System

The API supports multiple business modules:
- **ADMIN** - Administrative operations
- **SHOP** - E-commerce operations (future)
- **CHAT** - Communication operations (future)

Each module has its own set of commands and queries that are dynamically discovered and routed.

## ?? Testing

### Manual Testing
- Use Swagger UI for interactive testing
- Test files available in project root
- Postman collections (to be created)

### Integration Testing
(To be implemented)

## ?? Deployment

### Docker Deployment

```bash
# Build image
docker build -t legal-api .

# Run container
docker run -d -p 8080:8080 --name legal-api-container legal-api
```

### Docker Compose

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
```

## ?? Development

### Adding New Controllers

1. Create controller class inheriting from `ControllerBase`
2. Add appropriate route attributes
3. Inject required services
4. Implement endpoint methods

### Adding New Modules

1. Define module in `ApplicationHelper.ModuleName` enum
2. Create module-specific handlers
3. Register handlers in dependency injection
4. Update API documentation

## ?? References

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr/)
- [Swagger/OpenAPI](https://swagger.io/docs/)
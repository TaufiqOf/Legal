# Legal API System

## Overview

The Legal API System is a modular .NET 8 web application built using Clean Architecture principles. It provides a robust foundation for legal document management and processing with a modular design that supports multiple business domains.

## ??? Architecture

This solution follows Clean Architecture and Domain-Driven Design (DDD) principles with the following layers:

- **API Layer** (`Legal.Api.WebApi`) - RESTful API endpoints and controllers
- **Application Layer** (`Legal.Application.Admin`) - Business logic and use cases
- **Service Layer** (`Legal.Service.*`) - Infrastructure services and repositories
- **Shared Layer** (`Legal.Shared.SharedModel`) - Common models and utilities
- **Migration Service** (`MigrationService`) - Database migration and seeding

## ?? Features

- **Modular Architecture** - Support for multiple business modules (Admin, Shop, Chat)
- **CQRS Pattern** - Command and Query separation with handlers
- **Authentication & Authorization** - JWT-based security with role-based access
- **File Upload Support** - Multi-part form data handling for file operations
- **PostgreSQL Database** - Entity Framework Core with PostgreSQL provider
- **Docker Support** - Containerized deployment ready
- **Real-time Communication** - SignalR integration for live updates
- **Image Processing** - SixLabors.ImageSharp for image manipulation
- **Video Processing** - FFMpegCore for video operations

## ?? Project Structure

```
Legal.Api.Solution/
??? Api/
?   ??? Legal.Api.WebApi/              # Main Web API project
??? Application/
?   ??? Legal.Application.Admin/       # Admin module business logic
??? Service/
?   ??? Legal.Service.Infrastructure/  # Core infrastructure services
?   ??? Legal.Service.Repository/      # Data access layer
?   ??? Legal.Service.Helper/          # Utility helpers
??? Shared/
?   ??? Legal.Shared.SharedModel/      # Shared models and DTOs
??? OtherServices/
    ??? MigrationService/              # Database migration service
```

## ??? Technologies

- **.NET 8** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM with PostgreSQL
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Input validation
- **Newtonsoft.Json** - JSON serialization
- **SignalR** - Real-time web functionality
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization

## ?? Prerequisites

- .NET 8 SDK
- PostgreSQL 12+
- Docker (optional)
- Visual Studio 2022 or VS Code

## ?? Getting Started

### 1. Clone the Repository
```bash
git clone [repository-url]
cd Legal.Api.Solution
```

### 2. Database Setup
```bash
# Update connection string in appsettings.json
# Run migrations
dotnet ef database update --project Application/Legal.Application.Admin
```

### 3. Run the Application
```bash
# Using .NET CLI
dotnet run --project Api/Legal.Api.WebApi

# Using Docker
docker-compose up -d
```

### 4. Access the API
- **API Base URL**: `https://localhost:5001`
- **Swagger Documentation**: `https://localhost:5001/swagger`

## ?? API Endpoints

### Command Endpoints
- `POST /api/Command/Execute/{moduleName}` - Execute business commands
- `POST /api/Command/Upload/{moduleName}` - File upload operations
- `GET /api/Command/ListAll/{moduleName}` - List available commands
- `GET /api/Command/Detail/{moduleName}/{name}` - Get command details

### Query Endpoints
- `POST /api/Query/Execute/{moduleName}` - Execute data queries
- `GET /api/Query/ListAll/{moduleName}` - List available queries
- `GET /api/Query/Detail/{moduleName}/{name}` - Get query details
- `POST /api/Query/download/{moduleName}` - Download files

### Public Endpoints
- Various public endpoints for unauthenticated access

## ?? Configuration

### Environment Variables
- `ConnectionStrings:Postgres` - PostgreSQL connection string
- `AllowedOrigins` - CORS allowed origins
- `MaxRequestBodySize` - Maximum request body size

### User Secrets
The application uses .NET User Secrets for sensitive configuration:
```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "your-connection-string"
```

## ??? Modules

### Admin Module
- User management
- Authentication and authorization
- System administration

### Shop Module (Future)
- E-commerce functionality
- Product management

### Chat Module (Future)
- Real-time messaging
- Communication features

## ?? Security

- **JWT Authentication** - Token-based authentication
- **Role-based Authorization** - Granular access control
- **CORS Configuration** - Cross-origin request handling
- **Input Validation** - FluentValidation for all inputs

## ?? Database

- **Provider**: PostgreSQL
- **ORM**: Entity Framework Core
- **Migrations**: Code-first approach
- **Naming Convention**: Snake case

## ?? Docker Support

The application includes Docker support with:
- Multi-stage Dockerfile
- Docker Compose configuration
- Linux container support

## ?? Testing

(Testing framework to be implemented)

## ?? Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## ?? License

[License information to be added]

## ?? Support

For issues and questions, please create an issue in the repository.

---

## Project Documentation

For detailed documentation of each project, see:
- [API Documentation](Api/Legal.Api.WebApi/README.md)
- [Admin Application Documentation](Application/Legal.Application.Admin/README.md)
- [Infrastructure Service Documentation](Service/Legal.Service.Infrastructure/README.md)
- [Repository Service Documentation](Service/Legal.Service.Repository/README.md)
- [Helper Service Documentation](Service/Legal.Service.Helper/README.md)
- [Shared Models Documentation](Shared/Legal.Shared.SharedModel/README.md)
- [Migration Service Documentation](OtherServices/MigrationService/README.md)
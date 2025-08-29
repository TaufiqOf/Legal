# Legal.Application.Admin

## Overview

The Admin Application layer contains the business logic and use cases for the administrative module of the Legal API System. This project implements the Command Query Responsibility Segregation (CQRS) pattern with Entity Framework Core for data persistence.

## ??? Architecture

This application layer follows Clean Architecture principles and implements:
- **Command Handlers** - Write operations and business logic
- **Query Handlers** - Read operations and data retrieval
- **Entity Framework Context** - Database context and migrations
- **DTOs and Mapping** - Data transfer objects and AutoMapper configuration
- **Services** - Business services and domain logic

## ?? Project Structure

```
Legal.Application.Admin/
??? CommandHandlers/
?   ??? RegistrationCommandHandler.cs
?   ??? LogInCommandHandler.cs
?   ??? ChangePasswordCommandHandler.cs
?   ??? UserCommands/
?       ??? DeleteUserCommandHandler.cs
?       ??? EditUserCommandHandler.cs
??? Services/
?   ??? RegistrationService.cs
?   ??? IRegistrationService.cs
??? EntityConfigurations/
?   ??? UserEntityTypeConfiguration.cs
?   ??? AuditableEntityTypeConfiguration.cs
??? Migrations/
?   ??? 20250829160713_init.cs
?   ??? 20250829160713_init.Designer.cs
?   ??? AdminDatabaseContextModelSnapshot.cs
??? Dtos/
?   ??? UserDto.cs
??? DataSeeder/
?   ??? public.user.json
??? AdminDatabaseContext.cs
??? DependencyInjection.cs
??? MappingConfig.cs
??? GlobalUsings.cs
??? README.md
```

## ?? Features

### User Management
- **User Registration** - Create new user accounts
- **User Authentication** - Login and password validation
- **User Profile Management** - Edit and delete user accounts
- **Password Management** - Change password functionality

### Data Seeding
- **Initial User Data** - Seed default users from JSON configuration
- **Database Initialization** - Automatic database setup

### Security
- **Password Hashing** - Secure password storage
- **JWT Token Generation** - Authentication token creation
- **Role-based Access** - User role management

## ??? Database Context

### AdminDatabaseContext

The main Entity Framework context that manages:
- **User entities** - User account data
- **Auditable entities** - Entities with audit trails
- **Snake case naming** - PostgreSQL naming convention
- **Migration history** - Dedicated migration table

```csharp
public class AdminDatabaseContext : ADatabaseContext
{
    // Entity sets and configuration
}
```

### Entity Configurations

#### UserEntityTypeConfiguration
- User table structure
- Constraints and indexes
- Relationships

#### AuditableEntityTypeConfiguration
- Common auditable fields
- Created/updated timestamps
- User tracking

## ?? Command Handlers

### RegistrationCommandHandler
Handles new user registration:
- Validates user input
- Checks for duplicate emails
- Creates user account
- Generates initial access token

### LogInCommandHandler
Handles user authentication:
- Validates credentials
- Verifies password hash
- Generates JWT token
- Returns user information

### ChangePasswordCommandHandler
Handles password changes:
- Validates current password
- Updates password hash
- Logs security event

### User Command Handlers

#### EditUserCommandHandler
- Updates user profile information
- Validates changes
- Maintains audit trail

#### DeleteUserCommandHandler
- Soft delete user accounts
- Maintains data integrity
- Logs deletion event

## ?? Query Handlers

(Query handlers to be implemented based on requirements)

## ??? Services

### RegistrationService
Encapsulates user registration business logic:
- Email uniqueness validation
- Password policy enforcement
- User data validation
- Account activation logic

## ?? Data Transfer Objects

### UserDto
Represents user data for API responses:
- User identification
- Profile information
- Role assignments
- Excludes sensitive data

## ??? AutoMapper Configuration

### MappingConfig
Defines object-to-object mappings:
- Entity to DTO mappings
- Command to entity mappings
- Reverse mappings where needed

```csharp
public static class MappingConfig
{
    public static IMapper RegisterMappings()
    {
        // Mapping profiles configuration
    }
}
```

## ??? Dependency Injection

### DependencyInjection Extension
Configures all dependencies for the Admin module:

```csharp
public static IServiceCollection AddExpenseApplicationLayer(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Service registrations
}
```

#### Registered Services
- **Entity Framework Context** - Database context factory
- **AutoMapper** - Object mapping service
- **Command/Query Handlers** - Business logic handlers
- **Domain Services** - Business services
- **Memory Cache** - Caching service

## ?? Database Migrations

### Initial Migration (20250829160713_init)
Creates the initial database schema:
- User tables
- Audit tables
- Indexes and constraints

### Migration Commands

```bash
# Add new migration
dotnet ef migrations add <MigrationName> --project Legal.Application.Admin

# Update database
dotnet ef database update --project Legal.Application.Admin

# Remove last migration
dotnet ef migrations remove --project Legal.Application.Admin
```

## ?? Data Seeding

### Default Users (public.user.json)
Contains initial user data for system setup:
- Administrative users
- Default roles
- Test accounts (development only)

### DataSeederService Integration
The seeding is integrated with the main API through the data seeder service.

## ?? Security Features

### Password Security
- **BCrypt Hashing** - Strong password hashing
- **Salt Generation** - Unique salt per password
- **Policy Enforcement** - Password complexity rules

### Authentication
- **JWT Tokens** - Stateless authentication
- **Role Claims** - Role-based authorization
- **Token Expiration** - Configurable token lifetime

## ?? Dependencies

### NuGet Packages

- **FFMpegCore** (5.1.0) - Video processing capabilities
- **Microsoft.AspNetCore.SignalR.*** - Real-time communication
- **EFCore.NamingConventions** (8.0.3) - Snake case naming
- **Microsoft.EntityFrameworkCore.Design** (8.0.13) - EF Core tools
- **SixLabors.ImageSharp** (3.1.11) - Image processing
- **Microsoft.Extensions.Configuration.UserSecrets** - Configuration management

### Project References

- **Legal.Service.Infrastructure** - Core infrastructure services
- **Legal.Service.Repository** - Data access layer
- **Legal.Shared.SharedModel** - Shared models and DTOs

## ?? Usage Examples

### User Registration

```csharp
public class RegistrationCommand : IParameterModel<IValidator>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

### User Login

```csharp
public class LogInCommand : IParameterModel<IValidator>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
```

## ?? Testing

### Unit Testing
(To be implemented)
- Command handler tests
- Service tests
- Validation tests

### Integration Testing
(To be implemented)
- Database integration tests
- End-to-end scenarios

## ?? Configuration

### Connection String
Configure PostgreSQL connection in appsettings.json:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Database=LegalDB;Username=user;Password=pass"
  }
}
```

### Module Registration
The module is registered in the main API startup:

```csharp
builder.Services.AddExpenseApplicationLayer(builder.Configuration);
```

## ?? Performance Considerations

### Database Optimization
- **Indexes** - Proper indexing on frequently queried columns
- **Connection Pooling** - EF Core connection pooling
- **Query Optimization** - Efficient LINQ queries

### Caching
- **Memory Cache** - Configured for frequently accessed data
- **Entity Tracking** - Optimized for read scenarios

## ?? Future Enhancements

- **Email Verification** - Account activation via email
- **Password Reset** - Forgot password functionality
- **Multi-factor Authentication** - Enhanced security
- **Audit Logging** - Comprehensive audit trails
- **User Roles Management** - Advanced role system

## ?? References

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [AutoMapper Documentation](https://docs.automapper.org/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
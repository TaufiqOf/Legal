# Legal.Service.Infrastructure

## Overview

The Infrastructure Service project provides the core infrastructure components and services for the Legal API System. This project contains base classes, interfaces, models, and services that are shared across all application modules.

## ??? Architecture

This infrastructure layer implements:
- **Base Models and Interfaces** - Common abstractions and contracts
- **Request/Response Pattern** - Standardized request handling
- **CQRS Infrastructure** - Command and Query base classes
- **Authentication Services** - JWT token handling and validation
- **Exception Handling** - Custom exceptions and error handling
- **Repository Pattern** - Data access abstractions

## ?? Project Structure

```
Legal.Service.Infrastructure/
??? Interface/
?   ??? IParameterModel.cs
?   ??? IRequestModel.cs
?   ??? IResponseModel.cs
?   ??? IResultModel.cs
?   ??? IHandler.cs
?   ??? IRequestHandler.cs
?   ??? IAccessToken.cs
?   ??? IBaseModel.cs
?   ??? IBaseRepository.cs
?   ??? IRepository.cs
?   ??? IDomainRepository.cs
??? Model/
?   ??? RequestModel.cs
?   ??? ResultModel.cs
?   ??? BaseModel.cs
?   ??? DomainBaseModel.cs
?   ??? ASqlModel.cs
?   ??? ACommandHandler.cs
?   ??? AQueryHandler.cs
?   ??? ADatabaseContext.cs
?   ??? RequestHandler.cs
?   ??? User.cs
?   ??? UserType.cs
?   ??? AccessToken.cs
?   ??? JwtBearerOptionsSettings.cs
?   ??? UploadFileParameterModel.cs
?   ??? BlankValidator.cs
??? Services/
?   ??? AuthorizeAttribute.cs
?   ??? TokenAuthorizeAttribute.cs
?   ??? Jwt.cs
??? Helper/
?   ??? JwtTokenGenerator.cs
?   ??? PasswordHasher.cs
?   ??? MapperHelper.cs
??? Exceptions/
?   ??? NotFoundException.cs
?   ??? ForbiddenException.cs
?   ??? UserDisabledException.cs
??? Events/
?   ??? AuthenticationJwtBearerEvents.cs
??? EntityConfigurations/
?   ??? BaseEntityConfiguration.cs
??? README.md
```

## ?? Core Interfaces

### IHandler<TParameter, TResponse>
Base interface for all command and query handlers:
```csharp
public interface IHandler<in TParameter, TResponse> : IRequestHandler
    where TParameter : IParameterModel<IValidator>
    where TResponse : IResponseModel
{
    Task<ResultModel<TResponse>> ExecuteHandler(
        RequestModel requestModel, 
        TParameter parameter, 
        CancellationToken cancellationToken);
}
```

### IParameterModel<TValidator>
Interface for request parameters with validation:
```csharp
public interface IParameterModel<out TValidator> where TValidator : IValidator
{
    TValidator Validator { get; }
    ModuleName ModuleName { get; set; }
}
```

### IRepository<TModel, TId>
Generic repository interface for data access:
```csharp
public interface IRepository<TModel, in TId> : IBaseRepository<TModel>
    where TModel : BaseModel<TId>
{
    // Repository methods
}
```

## ??? Base Models

### BaseModel<TId>
Base class for all entities:
- Generic ID support
- Common properties
- Audit trail support

### DomainBaseModel<TId>
Extended base model for domain entities:
- Domain-specific properties
- Business rule enforcement

### ASqlModel
Base class for SQL-based models:
- SQL generation support
- Parameter binding

## ?? CQRS Infrastructure

### ACommandHandler<TParameter, TResponse>
Abstract base class for command handlers:
- Write operations
- Business logic execution
- Transaction support
- Validation pipeline

### AQueryHandler<TParameter, TResponse>
Abstract base class for query handlers:
- Read operations
- Data retrieval
- Performance optimization
- Caching support

## ?? Authentication & Authorization

### JWT Services

#### JwtTokenGenerator
Generates and validates JWT tokens:
- Token creation
- Claims management
- Expiration handling
- Signature validation

#### AccessToken
Represents the current user's access token:
- User identification
- Role information
- Token metadata

### Authorization Attributes

#### AuthorizeAttribute
Custom authorization attribute:
- Role-based access control
- Permission validation
- Custom authorization logic

#### TokenAuthorizeAttribute
Token-specific authorization:
- JWT token validation
- Token expiration checks
- User status verification

## ?? Request Processing

### RequestHandler
Central request processing service:
- Command routing
- Query routing
- Validation orchestration
- Error handling
- Module discovery

#### Key Features
- **Dynamic Handler Discovery** - Automatic handler registration
- **Validation Pipeline** - FluentValidation integration
- **File Upload Support** - Multipart form data handling
- **Error Management** - Centralized exception handling

### RequestModel
Standard request wrapper:
- Request identification
- Timestamp tracking
- Module routing
- Parameter containment

### ResultModel<T>
Standard response wrapper:
- Success/failure indication
- Result data
- Error messages
- Metadata

## ??? Exception Handling

### Custom Exceptions

#### NotFoundException
Thrown when requested resources are not found:
```csharp
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
```

#### ForbiddenException
Thrown when access is denied:
```csharp
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}
```

#### UserDisabledException
Thrown when disabled users attempt access:
```csharp
public class UserDisabledException : Exception
{
    public UserDisabledException(string message) : base(message) { }
}
```

## ?? Helper Services

### PasswordHasher
Secure password hashing service:
- BCrypt implementation
- Salt generation
- Hash verification
- Security best practices

### MapperHelper
AutoMapper utility methods:
- Mapping configuration
- Profile registration
- Custom converters

## ??? Database Infrastructure

### ADatabaseContext
Abstract base database context:
- Common entity configurations
- Audit trail implementation
- Change tracking
- Query optimization

### BaseEntityConfiguration<T>
Base entity type configuration:
- Common column mappings
- Index definitions
- Constraint setup

## ?? File Upload Support

### UploadFileParameterModel
Specialized parameter model for file uploads:
- Form collection access
- File validation
- Multi-file support
- Size limitations

## ?? Validation

### BlankValidator
Default validator for parameters without specific validation:
- Always valid
- Placeholder implementation
- Interface compliance

## ?? Usage Examples

### Creating a Command Handler

```csharp
public class CreateUserCommandHandler : ACommandHandler<CreateUserParameter, UserResponse>
{
    public override async Task<ResultModel<UserResponse>> ExecuteHandler(
        RequestModel requestModel,
        CreateUserParameter parameter,
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Creating a Query Handler

```csharp
public class GetUserQueryHandler : AQueryHandler<GetUserParameter, UserResponse>
{
    public override async Task<ResultModel<UserResponse>> ExecuteHandler(
        RequestModel requestModel,
        GetUserParameter parameter,
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Using the Repository Pattern

```csharp
public class UserService
{
    private readonly IRepository<User, int> _userRepository;
    
    public UserService(IRepository<User, int> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User> GetUserAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
}
```

## ?? Configuration

### JWT Configuration

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "legal-api",
    "Audience": "legal-client",
    "ExpirationMinutes": 60
  }
}
```

### Database Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "connection-string"
  }
}
```

## ?? Dependencies

### Core Dependencies
- **FluentValidation** - Input validation
- **Microsoft.AspNetCore.*** - ASP.NET Core components
- **Microsoft.EntityFrameworkCore** - ORM framework
- **Newtonsoft.Json** - JSON serialization
- **AutoMapper** - Object mapping

### Authentication Dependencies
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication
- **System.IdentityModel.Tokens.Jwt** - JWT token handling

## ?? Design Patterns

### Repository Pattern
- **Abstraction** - Data access abstraction
- **Testability** - Easy unit testing
- **Consistency** - Uniform data access

### CQRS Pattern
- **Separation** - Command/Query separation
- **Scalability** - Independent scaling
- **Optimization** - Operation-specific optimization

### Mediator Pattern
- **Decoupling** - Loose coupling between components
- **Single Responsibility** - Focused handlers
- **Extensibility** - Easy to add new operations

## ?? Request Flow

1. **Request Reception** - API controller receives request
2. **Handler Discovery** - RequestHandler finds appropriate handler
3. **Validation** - Parameter validation using FluentValidation
4. **Authorization** - Token and permission validation
5. **Execution** - Handler executes business logic
6. **Response** - Standardized response returned

## ?? Testing

### Unit Testing Guidelines
- **Mock Dependencies** - Use interfaces for mocking
- **Test Handlers** - Test command/query handlers independently
- **Validate Results** - Test ResultModel responses

### Integration Testing
- **Database Tests** - Test repository implementations
- **Handler Tests** - Test complete request flow
- **Authentication Tests** - Test JWT token handling

## ?? Performance Considerations

### Optimization Strategies
- **Async/Await** - Asynchronous operations throughout
- **Connection Pooling** - Database connection optimization
- **Caching** - Strategic caching implementation
- **Lazy Loading** - Deferred data loading

### Monitoring
- **Logging** - Comprehensive logging strategy
- **Metrics** - Performance metrics collection
- **Tracing** - Request tracing support

## ?? Future Enhancements

- **Event Sourcing** - Event-driven architecture
- **Distributed Caching** - Redis integration
- **Message Queuing** - Asynchronous message processing
- **Health Checks** - System health monitoring

## ?? References

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [JWT Authentication](https://jwt.io/introduction/)
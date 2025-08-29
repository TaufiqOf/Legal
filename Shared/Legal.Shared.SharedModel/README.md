# Legal.Shared.SharedModel

## Overview

The Shared Models project contains common data transfer objects (DTOs), parameter models, response models, and validators that are shared across all modules in the Legal API System. This project provides standardized contracts for API communication and data validation.

## ??? Architecture

This shared library implements:
- **Parameter Models** - Input models for commands and queries
- **Response Models** - Output models for API responses
- **Validators** - FluentValidation validators for input validation
- **SQL Models** - Database operation models
- **Base Classes** - Common base implementations

## ?? Project Structure

```
Legal.Shared.SharedModel/
??? ParameterModel/
?   ??? BaseParameterModel.cs         # Base class for all parameter models
?   ??? LogInParameterModel.cs        # Login request parameters
?   ??? RegistrationParameterModel.cs # User registration parameters
?   ??? EditUserParameterModel.cs     # User edit parameters
?   ??? DeleteParameterModel.cs       # Delete operation parameters
?   ??? BulkDeleteParameterModel.cs   # Bulk delete parameters
?   ??? IdParameterModel.cs           # Single ID parameter
?   ??? GetParameterModel.cs          # Get single item parameters
?   ??? GetItemsParameterModel.cs     # Get multiple items parameters
?   ??? ResetPasswordParameterModel.cs # Password reset parameters
?   ??? RestoreParameterModel.cs      # Restore operation parameters
??? ResponseModel/
?   ??? EmptyResponseModel.cs         # Empty response
?   ??? GetAllResponseModel.cs        # Collection response
?   ??? PagedResponseModel.cs         # Paginated response
?   ??? FileResponseModel.cs          # File download response
?   ??? DownloadResponseModel.cs      # Download operation response
?   ??? AttachmentResponseModel.cs    # File attachment response
?   ??? NotificationResponseModel.cs  # Notification response
?   ??? User/
?       ??? UserResponseModel.cs      # User data response
?       ??? UserTypeResponseModel.cs  # User type response
??? Validator/
?   ??? GetValidator.cs               # Validation for get operations
?   ??? GetItemsValidator.cs          # Validation for get multiple operations
??? SqlModels/
?   ??? InsertSqlModel.cs             # SQL insert model
?   ??? UpdateSqlModel.cs             # SQL update model
?   ??? GetIdsInDBSqlModel.cs         # SQL ID lookup model
??? README.md                         # This file
```

## ?? Parameter Models

### BaseParameterModel
Abstract base class for all parameter models:
```csharp
public abstract record BaseParameterModel : IParameterModel<IValidator>
{
    public virtual ApplicationHelper.ModuleName ModuleName { get; set; }
    public virtual IValidator Validator => new BlankValidator();
}
```

**Features:**
- **Module Association** - Links parameters to specific modules
- **Validation Support** - Built-in validator property
- **Record Type** - Immutable parameter objects
- **Inheritance Ready** - Base for all parameter models

### Authentication Parameters

#### LogInParameterModel
Parameters for user authentication:
```csharp
public record LogInParameterModel : BaseParameterModel
{
    public string Email { get; init; }
    public string Password { get; init; }
    // Validation rules included
}
```

#### RegistrationParameterModel
Parameters for user registration:
```csharp
public record RegistrationParameterModel : BaseParameterModel
{
    public string Email { get; init; }
    public string Password { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? PhoneNumber { get; init; }
    // Additional registration fields
}
```

#### ResetPasswordParameterModel
Parameters for password reset operations:
```csharp
public record ResetPasswordParameterModel : BaseParameterModel
{
    public string Email { get; init; }
    public string Token { get; init; }
    public string NewPassword { get; init; }
}
```

### CRUD Parameters

#### IdParameterModel
Simple ID-based parameter:
```csharp
public record IdParameterModel : BaseParameterModel
{
    public int Id { get; init; }
}
```

#### GetParameterModel
Extended get operation parameters:
```csharp
public record GetParameterModel : BaseParameterModel
{
    public int Id { get; init; }
    public bool IncludeRelated { get; init; }
    public string[]? Fields { get; init; }
}
```

#### GetItemsParameterModel
Parameters for retrieving multiple items:
```csharp
public record GetItemsParameterModel : BaseParameterModel
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
    public Dictionary<string, object>? Filters { get; init; }
}
```

#### DeleteParameterModel
Parameters for delete operations:
```csharp
public record DeleteParameterModel : BaseParameterModel
{
    public int Id { get; init; }
    public bool SoftDelete { get; init; } = true;
    public string? Reason { get; init; }
}
```

#### BulkDeleteParameterModel
Parameters for bulk delete operations:
```csharp
public record BulkDeleteParameterModel : BaseParameterModel
{
    public int[] Ids { get; init; }
    public bool SoftDelete { get; init; } = true;
    public string? Reason { get; init; }
}
```

### User Management Parameters

#### EditUserParameterModel
Parameters for user profile updates:
```csharp
public record EditUserParameterModel : BaseParameterModel
{
    public int UserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public bool? IsActive { get; init; }
    public string[]? Roles { get; init; }
}
```

## ?? Response Models

### EmptyResponseModel
Used when no data needs to be returned:
```csharp
public record EmptyResponseModel : IResponseModel
{
    // Empty response for operations that don't return data
}
```

### Collection Responses

#### GetAllResponseModel<T>
Generic response for collections:
```csharp
public record GetAllResponseModel<T> : IResponseModel
{
    public IEnumerable<T> Items { get; init; }
    public int TotalCount { get; init; }
    public DateTime RetrievedAt { get; init; }
}
```

#### PagedResponseModel<T>
Paginated collection response:
```csharp
public record PagedResponseModel<T> : IResponseModel
{
    public IEnumerable<T> Items { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
}
```

### File Responses

#### FileResponseModel
Response for file downloads:
```csharp
public record FileResponseModel : IResponseModel
{
    public Stream FileStream { get; init; }
    public string FileName { get; init; }
    public string ContentType { get; init; }
    public long FileSize { get; init; }
}
```

#### DownloadResponseModel
Enhanced download response:
```csharp
public record DownloadResponseModel : IResponseModel
{
    public byte[] FileContent { get; init; }
    public string FileName { get; init; }
    public string ContentType { get; init; }
    public Dictionary<string, string>? Headers { get; init; }
}
```

#### AttachmentResponseModel
File attachment response:
```csharp
public record AttachmentResponseModel : IResponseModel
{
    public string AttachmentId { get; init; }
    public string FileName { get; init; }
    public string FileUrl { get; init; }
    public long FileSize { get; init; }
    public string ContentType { get; init; }
    public DateTime UploadedAt { get; init; }
}
```

### User Responses

#### UserResponseModel
User information response:
```csharp
public record UserResponseModel : IResponseModel
{
    public int Id { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; }
    public string[] Roles { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
}
```

#### UserTypeResponseModel
User type/role information:
```csharp
public record UserTypeResponseModel : IResponseModel
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string[] Permissions { get; init; }
    public bool IsActive { get; init; }
}
```

### Notification Response

#### NotificationResponseModel
System notification response:
```csharp
public record NotificationResponseModel : IResponseModel
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string Message { get; init; }
    public string Type { get; init; } // Info, Warning, Error, Success
    public DateTime CreatedAt { get; init; }
    public bool IsRead { get; init; }
    public string? ActionUrl { get; init; }
}
```

## ? Validators

### GetValidator
Validates get operation parameters:
```csharp
public class GetValidator : AbstractValidator<GetParameterModel>
{
    public GetValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");
    }
}
```

### GetItemsValidator
Validates get items operation parameters:
```csharp
public class GetItemsValidator : AbstractValidator<GetItemsParameterModel>
{
    public GetItemsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");
            
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must be between 1 and 100");
    }
}
```

## ??? SQL Models

### InsertSqlModel
Base model for SQL insert operations:
```csharp
public abstract record InsertSqlModel : ASqlModel
{
    // SQL generation for insert operations
    public abstract string GenerateInsertSql();
    public abstract object GetParameters();
}
```

### UpdateSqlModel
Base model for SQL update operations:
```csharp
public abstract record UpdateSqlModel : ASqlModel
{
    // SQL generation for update operations
    public abstract string GenerateUpdateSql();
    public abstract object GetParameters();
}
```

### GetIdsInDBSqlModel
Model for ID existence checks:
```csharp
public record GetIdsInDBSqlModel : ASqlModel
{
    public int[] Ids { get; init; }
    public string TableName { get; init; }
    public string IdColumnName { get; init; } = "id";
}
```

## ?? Usage Examples

### Command Handler Usage

```csharp
public class LoginCommandHandler : ACommandHandler<LogInParameterModel, UserResponseModel>
{
    public override async Task<ResultModel<UserResponseModel>> ExecuteHandler(
        RequestModel requestModel,
        LogInParameterModel parameter,
        CancellationToken cancellationToken)
    {
        // Use the parameter model
        var user = await authenticateUser(parameter.Email, parameter.Password);
        
        // Return response model
        return new ResultModel<UserResponseModel>
        {
            Success = true,
            Result = new UserResponseModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
                // ... other properties
            }
        };
    }
}
```

### Query Handler Usage

```csharp
public class GetUsersQueryHandler : AQueryHandler<GetItemsParameterModel, PagedResponseModel<UserResponseModel>>
{
    public override async Task<ResultModel<PagedResponseModel<UserResponseModel>>> ExecuteHandler(
        RequestModel requestModel,
        GetItemsParameterModel parameter,
        CancellationToken cancellationToken)
    {
        // Use pagination parameters
        var users = await _userService.GetPagedUsersAsync(
            parameter.Page,
            parameter.PageSize,
            parameter.SearchTerm,
            cancellationToken);
        
        // Return paged response
        return new ResultModel<PagedResponseModel<UserResponseModel>>
        {
            Success = true,
            Result = new PagedResponseModel<UserResponseModel>
            {
                Items = users.Items.Select(u => new UserResponseModel { /* mapping */ }),
                Page = parameter.Page,
                PageSize = parameter.PageSize,
                TotalCount = users.TotalCount,
                TotalPages = (int)Math.Ceiling((double)users.TotalCount / parameter.PageSize)
            }
        };
    }
}
```

### API Controller Usage

```csharp
[HttpPost("login")]
public async Task<ActionResult<UserResponseModel>> Login([FromBody] LogInParameterModel model)
{
    var request = new RequestModel
    {
        RequestName = "LogIn",
        Parameter = model
    };
    
    var result = await _requestHandler.Execute("ADMIN", JObject.FromObject(request), cancellationToken);
    
    return result.Success ? Ok(result.Result) : BadRequest(result);
}
```

## ?? Validation Integration

### Automatic Validation
All parameter models include validation through the Validator property:

```csharp
public record CreateUserParameterModel : BaseParameterModel
{
    public string Email { get; init; }
    public string Password { get; init; }
    
    public override IValidator Validator => new CreateUserValidator();
}

public class CreateUserValidator : AbstractValidator<CreateUserParameterModel>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid email is required");
            
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage("Password must contain uppercase, lowercase, and number");
    }
}
```

## ?? Dependencies

### Core Dependencies
- **FluentValidation** - Input validation framework
- **Legal.Service.Infrastructure** - Base interfaces and models
- **Legal.Service.Helper** - Helper utilities

### Design Principles
- **Immutability** - Record types for immutable data
- **Validation** - Built-in validation support
- **Standardization** - Consistent model structure
- **Reusability** - Shared across all modules

## ?? Best Practices

### Parameter Model Design
- **Use Records** - Immutable data structures
- **Include Validation** - Always provide validator
- **Descriptive Names** - Clear and specific naming
- **Optional Properties** - Use nullable types for optional data

### Response Model Design
- **Implement Interface** - Always implement IResponseModel
- **Include Metadata** - Timestamps, counts, etc.
- **Consistent Structure** - Follow established patterns
- **Documentation** - XML documentation for all properties

### Validation Rules
- **Clear Messages** - Descriptive error messages
- **Business Rules** - Enforce business constraints
- **Security** - Validate for security concerns
- **Performance** - Efficient validation rules

## ?? Testing

### Model Validation Testing

```csharp
[Test]
public void LogInParameterModel_ValidInput_ShouldPassValidation()
{
    // Arrange
    var model = new LogInParameterModel
    {
        Email = "test@example.com",
        Password = "ValidPassword123"
    };
    
    // Act
    var validationResult = model.Validator.Validate(model);
    
    // Assert
    Assert.IsTrue(validationResult.IsValid);
}

[Test]
public void LogInParameterModel_InvalidEmail_ShouldFailValidation()
{
    // Arrange
    var model = new LogInParameterModel
    {
        Email = "invalid-email",
        Password = "ValidPassword123"
    };
    
    // Act
    var validationResult = model.Validator.Validate(model);
    
    // Assert
    Assert.IsFalse(validationResult.IsValid);
    Assert.IsTrue(validationResult.Errors.Any(e => e.PropertyName == "Email"));
}
```

## ?? Future Enhancements

- **Versioning Support** - Model versioning for API evolution
- **Custom Validators** - Domain-specific validation rules
- **Serialization Attributes** - JSON serialization customization
- **Documentation Attributes** - Enhanced API documentation
- **Localization** - Multi-language validation messages

## ?? References

- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [C# Records](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#record-types)
- [Data Transfer Objects](https://martinfowler.com/eaaCatalog/dataTransferObject.html)
- [API Design Guidelines](https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design)
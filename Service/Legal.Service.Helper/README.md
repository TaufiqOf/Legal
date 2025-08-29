# Legal.Service.Helper

## Overview

The Helper Service project provides utility classes and helper methods that are commonly used across the Legal API System. This project contains reusable helper functions, constants, and utility services that support various operations throughout the application.

## ??? Architecture

This helper library implements:
- **Service Registration Utilities** - Automatic service discovery and registration
- **Directory Management** - File system operations and path management
- **Application Configuration** - Application-wide constants and settings
- **Common Utilities** - Reusable utility functions

## ?? Project Structure

```
Legal.Service.Helper/
??? ApplicationHelper.cs           # Application-wide settings and enums
??? CommonHelper.cs               # Service registration and common utilities
??? DirectoryHelper.cs            # File system and directory operations
??? ConnectionStringsConstants.cs # Database connection string constants
??? README.md                     # This file
```

## ?? Helper Classes

### ApplicationHelper
Central application configuration and constants:

#### ModuleName Enum
Defines the available business modules in the system:
```csharp
public enum ModuleName
{
    ADMIN = 0,    // Administrative module
    SHOP = 1,     // E-commerce module (future)
    CHAT = 2,     // Communication module (future)
}
```

#### Application Properties
- **ApplicationName** - Gets or sets the application name
- **ApplicationVersion** - Gets the current application version from assembly

### CommonHelper
Service registration and dependency injection utilities:

#### Handler Registration
Automatically discovers and registers command/query handlers:
```csharp
public static IEnumerable<Type> RegisterHandlers(
    this IServiceCollection services,
    Assembly assembly,
    params Type[] types)
```

**Features:**
- **Automatic Discovery** - Finds handlers by base type
- **Generic Support** - Works with generic handler base classes
- **Scoped Lifetime** - Registers handlers with scoped lifetime
- **Type Filtering** - Filters handlers by specified base types

#### Type Registration
Registers types implementing interfaces automatically:
```csharp
public static IServiceCollection RegisterTypes(
    this IServiceCollection services,
    Assembly assembly,
    params Type[] typesToIgnore)
```

**Features:**
- **Interface Implementation** - Registers types as their implemented interfaces
- **Assembly Scanning** - Scans entire assemblies for types
- **Exclusion Support** - Excludes specified types from registration
- **Automatic Lifetime** - Uses scoped lifetime by default

### DirectoryHelper
File system and directory management utilities:

#### Storage Path Management
```csharp
public static string GetStoragePath(string? folder = null)
```
- Gets application-specific storage directory in AppData
- Creates directory if it doesn't exist
- Supports subfolder creation
- Returns platform-appropriate path

#### Temporary Path Management
```csharp
public static string GetApplicationTemporaryPath(string? folder = null)
public static string GetTemporaryPath(string? folder = null)
```
- Gets temporary directory paths
- Application-specific or system temporary paths
- Automatic directory creation
- Cross-platform compatibility

### ConnectionStringsConstants
Database and service connection string constants:

```csharp
public static class ConnectionStringsConstants
{
    public const string Postgres = "Postgres";
    public const string Redis = "Redis";
}
```

**Purpose:**
- **Standardization** - Consistent connection string naming
- **Maintainability** - Centralized constant management
- **Type Safety** - Prevents string literal errors

## ?? Usage Examples

### Service Registration with CommonHelper

```csharp
// In DependencyInjection.cs or Startup.cs
public static IServiceCollection AddApplicationLayer(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Register handlers automatically
    var handlers = services.RegisterHandlers(
        Assembly.GetExecutingAssembly(),
        typeof(ACommandHandler<,>),
        typeof(AQueryHandler<,>));

    // Register other types automatically
    services.RegisterTypes(
        Assembly.GetExecutingAssembly(),
        typeof(IBaseModel),
        typeof(IEntityTypeConfiguration<>));

    return services;
}
```

### Directory Operations with DirectoryHelper

```csharp
// Get application storage directory
var appStoragePath = DirectoryHelper.GetStoragePath();
// Result: C:\Users\{User}\AppData\Roaming\{ApplicationName}

// Get application storage subdirectory
var uploadsPath = DirectoryHelper.GetStoragePath("uploads");
// Result: C:\Users\{User}\AppData\Roaming\{ApplicationName}\uploads

// Get temporary directory for application
var tempPath = DirectoryHelper.GetApplicationTemporaryPath();
// Result: C:\Users\{User}\AppData\Local\Temp\{ApplicationName}

// Get temporary subdirectory
var tempUploads = DirectoryHelper.GetApplicationTemporaryPath("processing");
// Result: C:\Users\{User}\AppData\Local\Temp\{ApplicationName}\processing
```

### Using Connection String Constants

```csharp
// In configuration or service registration
var connectionString = configuration.GetConnectionString(ConnectionStringsConstants.Postgres);

services.AddDbContext<YourDbContext>(options =>
    options.UseNpgsql(connectionString));

// Redis connection
var redisConnection = configuration.GetConnectionString(ConnectionStringsConstants.Redis);
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
});
```

### Application Information Usage

```csharp
// Set application name (typically in Program.cs)
ApplicationHelper.ApplicationName = "Legal API System";

// Get application version
var version = ApplicationHelper.ApplicationVersion;
Console.WriteLine($"Application Version: {version}");

// Use module names
var adminModule = ModuleName.ADMIN;
var moduleString = adminModule.ToString(); // "ADMIN"
```

## ?? Configuration Integration

### Dependency Injection Setup

```csharp
// Example usage in Program.cs or Startup.cs
public static void Main(string[] args)
{
    // Set application name
    ApplicationHelper.ApplicationName = "Legal API System";
    
    var builder = WebApplication.CreateBuilder(args);
    
    // Use helpers for service registration
    builder.Services.AddApplicationServices();
    
    var app = builder.Build();
    app.Run();
}

public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // Use CommonHelper for automatic registration
    services.RegisterHandlers(
        Assembly.GetExecutingAssembly(),
        typeof(ACommandHandler<,>),
        typeof(AQueryHandler<,>));
        
    return services;
}
```

### File System Configuration

```csharp
// Configure file storage locations
public class FileStorageService
{
    private readonly string _uploadsPath;
    private readonly string _tempPath;
    
    public FileStorageService()
    {
        _uploadsPath = DirectoryHelper.GetStoragePath("uploads");
        _tempPath = DirectoryHelper.GetApplicationTemporaryPath("processing");
    }
    
    public async Task<string> SaveFileAsync(IFormFile file)
    {
        var fileName = Path.GetRandomFileName();
        var filePath = Path.Combine(_uploadsPath, fileName);
        
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        
        return filePath;
    }
}
```

## ?? Module Management

### Module Enumeration
The ModuleName enum provides a standardized way to identify different business modules:

```csharp
public class ModuleService
{
    public string GetModuleDisplayName(ModuleName module)
    {
        return module switch
        {
            ModuleName.ADMIN => "Administration",
            ModuleName.SHOP => "E-Commerce",
            ModuleName.CHAT => "Communication",
            _ => "Unknown Module"
        };
    }
    
    public bool IsModuleEnabled(ModuleName module)
    {
        // Check if module is enabled based on configuration
        return module switch
        {
            ModuleName.ADMIN => true,  // Always enabled
            ModuleName.SHOP => false,  // Future implementation
            ModuleName.CHAT => false,  // Future implementation
            _ => false
        };
    }
}
```

## ?? Design Patterns

### Extension Methods
CommonHelper uses extension methods for fluent service registration:
- **Fluent API** - Chainable method calls
- **Readability** - Clear and readable service configuration
- **Consistency** - Consistent pattern across all helpers

### Static Utilities
Helper classes are designed as static utilities:
- **No State** - Stateless utility functions
- **Performance** - No object instantiation overhead
- **Simplicity** - Simple method calls without dependency injection

### Constants Class
ConnectionStringsConstants follows the constants class pattern:
- **Type Safety** - Compile-time checking
- **Intellisense** - IDE auto-completion support
- **Maintainability** - Single location for constant updates

## ?? Testing

### Unit Testing Helper Methods

```csharp
[Test]
public void GetStoragePath_ShouldCreateDirectory()
{
    // Arrange
    ApplicationHelper.ApplicationName = "TestApp";
    var testFolder = "test-folder";
    
    // Act
    var path = DirectoryHelper.GetStoragePath(testFolder);
    
    // Assert
    Assert.IsTrue(Directory.Exists(path));
    Assert.IsTrue(path.Contains("TestApp"));
    Assert.IsTrue(path.Contains(testFolder));
}

[Test]
public void RegisterHandlers_ShouldRegisterHandlerTypes()
{
    // Arrange
    var services = new ServiceCollection();
    var assembly = Assembly.GetExecutingAssembly();
    
    // Act
    var handlerTypes = services.RegisterHandlers(assembly, typeof(ACommandHandler<,>));
    
    // Assert
    Assert.IsTrue(handlerTypes.Any());
    // Verify specific handler registrations
}
```

### Integration Testing
- **Directory Operations** - Test actual file system operations
- **Service Registration** - Verify dependency injection setup
- **Cross-Platform** - Test on different operating systems

## ?? Dependencies

### Core Dependencies
- **Microsoft.Extensions.DependencyInjection** - Dependency injection extensions
- **System.IO** - File system operations
- **System.Reflection** - Assembly scanning and type discovery

### No External Dependencies
This helper library intentionally has minimal dependencies to:
- **Reduce Complexity** - Keep the helper library lightweight
- **Avoid Conflicts** - Prevent dependency version conflicts
- **Improve Performance** - Minimize assembly loading overhead

## ?? Performance Considerations

### Assembly Scanning
- **Caching** - Assembly scanning results can be cached
- **Lazy Loading** - Only scan assemblies when needed
- **Efficient Filtering** - Use efficient type filtering predicates

### Directory Operations
- **Path Caching** - Cache frequently used directory paths
- **Batch Operations** - Group multiple directory operations
- **Exception Handling** - Handle file system exceptions gracefully

## ?? Future Enhancements

- **Configuration Helper** - Configuration management utilities
- **Validation Helper** - Common validation methods
- **Serialization Helper** - JSON/XML serialization utilities
- **Logging Helper** - Structured logging utilities
- **Caching Helper** - Caching operation utilities

## ?? References

- [.NET Dependency Injection](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [File System Operations](https://docs.microsoft.com/en-us/dotnet/api/system.io)
- [Assembly Reflection](https://docs.microsoft.com/en-us/dotnet/api/system.reflection)
- [Extension Methods](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)
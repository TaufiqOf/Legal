# Legal.Service.Repository

## Overview

The Repository Service project provides the data access layer implementation for the Legal API System. This project implements the Repository pattern using Entity Framework Core with PostgreSQL, providing concrete implementations of the repository interfaces defined in the Infrastructure layer.

## ??? Architecture

This repository layer implements:
- **Generic Repository Pattern** - Reusable data access operations
- **PostgreSQL Integration** - Entity Framework Core with PostgreSQL provider
- **Domain Repository Support** - Specialized domain-specific repositories
- **Abstract Base Classes** - Common repository functionality
- **Type-safe Operations** - Strongly-typed data access methods

## ?? Project Structure

```
Legal.Service.Repository/
??? PostgresRepository.cs          # Generic PostgreSQL repository implementation
??? DomainPostgresRepository.cs    # Domain-specific PostgreSQL repository
??? APostgresRepository.cs         # Abstract base PostgreSQL repository
??? README.md                      # This file
```

## ??? Repository Implementations

### PostgresRepository<TModel, TId>
Generic repository implementation for PostgreSQL:
- **CRUD Operations** - Create, Read, Update, Delete
- **Async Support** - All operations are asynchronous
- **Entity Framework Integration** - Uses EF Core DbContext
- **Generic Type Support** - Works with any entity type
- **Query Optimization** - Efficient database queries

#### Key Features
- Generic entity support with type safety
- Bulk operations for performance
- Transaction support
- Connection management
- Query expression support

### DomainPostgresRepository<TModel, TId>
Specialized repository for domain entities:
- **Domain Logic** - Business rule enforcement
- **Advanced Queries** - Complex domain-specific queries
- **Aggregate Support** - Domain aggregate handling
- **Business Validation** - Domain rule validation

### APostgresRepository<TModel, TId>
Abstract base class providing common functionality:
- **Shared Operations** - Common database operations
- **Connection Management** - Database connection handling
- **Error Handling** - Standardized error management
- **Logging Support** - Operation logging

## ?? Core Operations

### Basic CRUD Operations

#### Create Operations
```csharp
Task<TModel> CreateAsync(TModel entity, CancellationToken cancellationToken = default);
Task<IEnumerable<TModel>> CreateRangeAsync(IEnumerable<TModel> entities, CancellationToken cancellationToken = default);
```

#### Read Operations
```csharp
Task<TModel?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken = default);
Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
Task<TModel?> FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
```

#### Update Operations
```csharp
Task<TModel> UpdateAsync(TModel entity, CancellationToken cancellationToken = default);
Task<IEnumerable<TModel>> UpdateRangeAsync(IEnumerable<TModel> entities, CancellationToken cancellationToken = default);
```

#### Delete Operations
```csharp
Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
Task DeleteAsync(TModel entity, CancellationToken cancellationToken = default);
Task DeleteRangeAsync(IEnumerable<TModel> entities, CancellationToken cancellationToken = default);
```

### Advanced Query Operations

#### Counting and Existence
```csharp
Task<int> CountAsync(CancellationToken cancellationToken = default);
Task<int> CountAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
Task<bool> ExistsAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
```

#### Pagination Support
```csharp
Task<PagedResult<TModel>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
Task<PagedResult<TModel>> GetPagedAsync(Expression<Func<TModel, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
```

#### Ordering and Filtering
```csharp
Task<IEnumerable<TModel>> GetOrderedAsync<TKey>(Expression<Func<TModel, TKey>> orderBy, bool ascending = true, CancellationToken cancellationToken = default);
Task<IEnumerable<TModel>> GetFilteredAsync(Expression<Func<TModel, bool>> filter, CancellationToken cancellationToken = default);
```

## ?? Usage Examples

### Basic Entity Operations

```csharp
// Dependency injection setup
services.AddTransient(typeof(IRepository<,>), typeof(PostgresRepository<,>));
services.AddTransient(typeof(IDomainRepository<,>), typeof(DomainPostgresRepository<,>));

// Usage in service class
public class UserService
{
    private readonly IRepository<User, int> _userRepository;
    
    public UserService(IRepository<User, int> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User> CreateUserAsync(User user)
    {
        return await _userRepository.CreateAsync(user);
    }
    
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
    
    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _userRepository.FindAsync(u => u.IsActive);
    }
}
```

### Domain Repository Usage

```csharp
public class UserDomainService
{
    private readonly IDomainRepository<User, int> _userRepository;
    
    public UserDomainService(IDomainRepository<User, int> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User> CreateUserWithValidationAsync(User user)
    {
        // Domain-specific validation and business rules
        return await _userRepository.CreateAsync(user);
    }
}
```

### Advanced Query Examples

```csharp
// Complex filtering
var activeUsers = await _userRepository.FindAsync(u => 
    u.IsActive && 
    u.CreatedDate > DateTime.UtcNow.AddDays(-30));

// Pagination
var pagedUsers = await _userRepository.GetPagedAsync(
    u => u.IsActive, 
    pageNumber: 1, 
    pageSize: 10);

// Ordering
var sortedUsers = await _userRepository.GetOrderedAsync(
    u => u.LastName, 
    ascending: true);

// Existence check
var userExists = await _userRepository.ExistsAsync(userId);

// Count operations
var activeUserCount = await _userRepository.CountAsync(u => u.IsActive);
```

## ?? Configuration

### Database Context Registration

```csharp
// In Startup.cs or Program.cs
services.AddDbContextFactory<YourDbContext>(options =>
{
    var connectionString = configuration.GetConnectionString("Postgres");
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention();
});
```

### Repository Registration

```csharp
// Generic repository registration
services.AddTransient(typeof(IRepository<,>), typeof(PostgresRepository<,>));
services.AddTransient(typeof(IDomainRepository<,>), typeof(DomainPostgresRepository<,>));

// Specific repository registration (if needed)
services.AddTransient<IRepository<User, int>, PostgresRepository<User, int>>();
```

## ?? Entity Framework Integration

### DbContext Usage
The repositories work with any Entity Framework DbContext:
- **Context Factory** - Uses DbContextFactory for proper lifetime management
- **Change Tracking** - Leverages EF Core change tracking
- **Lazy Loading** - Supports lazy loading when configured
- **Include Support** - Navigation property loading

### Query Optimization
- **No-Tracking Queries** - For read-only operations
- **Compiled Queries** - Performance optimization
- **Batch Operations** - Bulk insert/update/delete
- **Connection Pooling** - Efficient connection management

## ?? Performance Features

### Asynchronous Operations
All repository methods are asynchronous:
- **Non-blocking** - Doesn't block calling thread
- **Scalability** - Better application scalability
- **Cancellation Support** - Proper cancellation token handling

### Bulk Operations
Support for bulk operations:
- **Batch Insert** - Multiple entity creation
- **Batch Update** - Multiple entity updates
- **Batch Delete** - Multiple entity deletion

### Query Optimization
- **Expression Trees** - Efficient LINQ query translation
- **Predicate Building** - Dynamic query construction
- **Index Utilization** - Database index optimization

## ??? Error Handling

### Exception Management
Repositories handle various error scenarios:
- **Connection Errors** - Database connectivity issues
- **Constraint Violations** - Database constraint errors
- **Concurrency Conflicts** - Optimistic concurrency handling
- **Timeout Issues** - Command timeout management

### Logging Integration
Comprehensive logging support:
- **Operation Logging** - Log all repository operations
- **Error Logging** - Detailed error information
- **Performance Logging** - Query execution time tracking

## ?? Testing

### Unit Testing
Repositories can be easily unit tested:
- **In-Memory Database** - Use EF Core in-memory provider
- **Mock DbContext** - Mock the database context
- **Test Data Setup** - Easy test data creation

```csharp
[Test]
public async Task CreateAsync_ShouldCreateEntity()
{
    // Arrange
    var options = new DbContextOptionsBuilder<TestDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDB")
        .Options;
        
    using var context = new TestDbContext(options);
    var repository = new PostgresRepository<User, int>(context);
    var user = new User { Name = "Test User" };
    
    // Act
    var result = await repository.CreateAsync(user);
    
    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual("Test User", result.Name);
}
```

### Integration Testing
Full database integration testing:
- **Test Containers** - Use TestContainers for PostgreSQL
- **Migration Testing** - Test database migrations
- **Performance Testing** - Query performance validation

## ?? Dependencies

### Core Dependencies
- **Microsoft.EntityFrameworkCore** - ORM framework
- **Npgsql.EntityFrameworkCore.PostgreSQL** - PostgreSQL provider
- **EFCore.NamingConventions** - Snake case naming

### Project References
- **Legal.Service.Infrastructure** - Base interfaces and models
- **Legal.Shared.SharedModel** - Shared models and DTOs

## ?? Transaction Support

### Unit of Work Pattern
While not explicitly implemented, repositories support transactions:
- **DbContext Transactions** - Use EF Core transaction support
- **SaveChanges Batching** - Multiple operations in single transaction
- **Rollback Support** - Transaction rollback on errors

```csharp
// Example transaction usage
using var transaction = await context.Database.BeginTransactionAsync();
try
{
    await _userRepository.CreateAsync(user1);
    await _userRepository.CreateAsync(user2);
    await context.SaveChangesAsync();
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

## ?? Performance Considerations

### Connection Management
- **Connection Pooling** - Automatic connection pooling
- **Context Lifetime** - Proper DbContext lifetime management
- **Resource Disposal** - Automatic resource cleanup

### Query Performance
- **Query Compilation** - EF Core query compilation caching
- **Parameter Caching** - SQL parameter caching
- **Execution Plan Reuse** - Database execution plan optimization

### Memory Usage
- **Change Tracking** - Minimal change tracking overhead
- **Entity Caching** - First-level entity caching
- **Garbage Collection** - Efficient memory usage

## ?? Future Enhancements

- **Caching Layer** - Redis caching integration
- **Read Replicas** - Read/write splitting support
- **Audit Trail** - Automatic audit logging
- **Soft Delete** - Soft delete implementation
- **Optimistic Concurrency** - Concurrency conflict resolution

## ?? References

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql Documentation](https://www.npgsql.org/efcore/)
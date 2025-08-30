# Legal Migration Service

## Overview

The Migration Service is a console application designed to manage database migrations and schema updates for the Legal API System. This service provides a command-line interface for applying Entity Framework Core migrations across multiple database contexts and modules.

## ⚙️ Architecture

This migration service implements:
- **Command Line Interface** - Parameter-driven migration execution
- **Multi-Context Support** - Manages multiple database contexts
- **Automated Migration** - Batch migration processing
- **Configuration Management** - Flexible connection string configuration
- **Assembly Discovery** - Automatic context discovery

## 📁 Project Structure

```
MigrationService/
├── ContextFactories/           # Database context factory implementations
├── Scripts/                    # SQL migration scripts (embedded resources)
├── Program.cs                  # Main entry point and CLI logic
├── Options.cs                  # Command line options model
├── Configuration.cs            # Configuration management
├── Migrator.cs                 # Migration execution logic
├── appsettings.json            # Default configuration settings
├── Dockerfile                  # Docker container configuration
└── README.md                   # This file
```

## ✨ Features

### 🖥️ Command Line Interface
- **Interactive Mode** - Prompt-based context selection
- **Batch Mode** - Non-interactive execution with parameters
- **Context Selection** - Choose specific contexts or all contexts
- **Connection Override** - Runtime connection string override

### 🔄 Migration Management
- **Multiple Contexts** - Support for multiple database contexts
- **Selective Migration** - Apply migrations to specific contexts
- **Bulk Operations** - Apply migrations to all contexts at once
- **Error Handling** - Comprehensive error reporting and rollback

### 🗄️ Database Support
- **PostgreSQL** - Primary database support
- **DbUp Integration** - SQL script-based migrations
- **Connection Pooling** - Efficient database connections
- **Distributed Locking** - Prevents concurrent migration conflicts

## 🛠️ Command Line Usage

### Basic Commands

#### Apply All Migrations (Interactive)
```bash
MigrationService.exe
```
This will:
1. Display available database contexts
2. Prompt for context selection
3. Ask for confirmation before applying migrations

#### Apply All Migrations (Non-Interactive)
```bash
MigrationService.exe --update-database "y" --context-number "-1"
```

#### Apply Specific Context Migration
```bash
MigrationService.exe --update-database "y" --context-number "1"
```

#### Apply Multiple Contexts
```bash
# Specific contexts (1, 2, 3)
MigrationService.exe --update-database "y" --context-number "1,2,3"

# Context range (1 through 4)
MigrationService.exe --update-database "y" --context-number "1-4"
```

#### Override Connection Strings
```bash
# Override auth database connection
MigrationService.exe --auth-db "Host=localhost;Database=LegalAuth;Username=user;Password=pass"

# Override data database connection
MigrationService.exe --data-db "Host=localhost;Database=LegalData;Username=user;Password=pass"
```

### Command Line Options

| Option | Description | Example |
|--------|-------------|---------|
| `--auth-db` | Override authentication database connection string | `--auth-db "connection-string"` |
| `--data-db` | Override data database connection string | `--data-db "connection-string"` |
| `--update-database` | Auto-confirm migration (y/n) | `--update-database "y"` |
| `--context-number` | Context selection (-1=all, number=specific) | `--context-number "1"` |

## 🔍 Database Context Discovery

### Automatic Discovery
The service automatically discovers database contexts by:
1. **Assembly Scanning** - Scans referenced assemblies
2. **Type Filtering** - Finds types inheriting from `ADatabaseContext`
3. **Dynamic Loading** - Loads contexts dynamically at runtime

### Supported Contexts
Current contexts include:
- **AdminDatabaseContext** - Administrative module context
- **Future Contexts** - Additional contexts as modules are added

### Context Selection Methods

#### All Contexts (-1)
```bash
--context-number "-1"
```
Applies migrations to all discovered contexts.

#### Single Context (1, 2, 3, ...)
```bash
--context-number "1"
```
Applies migrations to the specified context number.

#### Multiple Contexts (1,2,3)
```bash
--context-number "1,2,3"
```
Applies migrations to contexts 1, 2, and 3.

#### Context Range (1-4)
```bash
--context-number "1-4"
```
Applies migrations to contexts 1 through 4.

## ⚙️ Configuration

### appsettings.json
Default configuration file:
```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Database=LegalDB;Username=postgres;Password=password;Port=5432",
    "Redis": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

### User Secrets
Sensitive configuration can be stored in User Secrets:
```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "your-production-connection-string"
```

### Environment Variables
Configuration can be overridden with environment variables:
- `ConnectionStrings__Postgres` - PostgreSQL connection string
- `ConnectionStrings__Redis` - Redis connection string

## 🧪 Migration Process

### Step-by-Step Process

1. **Configuration Loading**
   - Load appsettings.json
   - Apply command line overrides
   - Validate connection strings
2. **Context Discovery**
   - Scan referenced assemblies
   - Find ADatabaseContext implementations
   - Display available contexts
3. **Context Selection**
   - Interactive prompt or CLI parameter
   - Validate selection
   - Prepare context list
4. **Migration Execution**
   - Create context factory
   - Initialize migrator
   - Apply pending migrations
   - Report results
5. **Completion**
   - Display success/failure status
   - Log migration details
   - Clean up resources

### Migration Execution Flow

```csharp
// Simplified migration flow
foreach (var contextType in selectedContexts)
{
    // Create context factory
    var factory = new DbContextFactory<TContext>(configuration);
    
    // Create migrator
    var migrator = new Migrator<TContext>(configuration, factory);
    
    // Execute migration
    var result = migrator.MigrateDb();
    
    // Handle result
    if (result.Success)
    {
        Console.WriteLine($"Migration completed for {contextType.Name}");
    }
    else
    {
        Console.WriteLine($"Migration failed for {contextType.Name}: {result.Error}");
    }
}
```

## 🐳 Docker Support

### Dockerfile
The service includes Docker support for containerized migrations:

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["OtherServices/MigrationService/MigrationService.csproj", "OtherServices/MigrationService/"]
RUN dotnet restore "OtherServices/MigrationService/MigrationService.csproj"
COPY . .
WORKDIR "/src/OtherServices/MigrationService"
RUN dotnet build "MigrationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MigrationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Legal.MigrationService.dll"]
```

### Docker Usage

```bash
# Build image
docker build -t legal-migration-service .

# Run migration
docker run --rm -e "ConnectionStrings__Postgres=your-connection-string" \
  legal-migration-service --update-database "y" --context-number "-1"
```

## 📜 SQL Scripts

### Embedded Scripts
SQL scripts are embedded as resources in the assembly:
```xml
<ItemGroup>
    <EmbeddedResource Include="Scripts/**/*.sql" />
</ItemGroup>
```

### Script Organization
```
Scripts/
├── Admin/
│   ├── V001__CreateUserTables.sql
│   ├── V002__AddUserIndexes.sql
│   └── V003__SeedDefaultUsers.sql
└── Common/
    ├── V001__CreateAuditTables.sql
    └── V002__CreateLoggingTables.sql
```

### Script Naming Convention
- **Version Number** - V001, V002, etc.
- **Description** - Clear description of changes
- **SQL Extension** - .sql file extension

## 🔐 Security Considerations

### Connection Strings
- **User Secrets** - Store sensitive connection strings securely
- **Environment Variables** - Use environment variables in production
- **Least Privilege** - Database user should have minimum required permissions

### Migration Safety
- **Backup First** - Always backup database before migrations
- **Test Environment** - Test migrations in staging environment
- **Rollback Plan** - Have rollback strategy for failed migrations

## 🧪 Testing Migrations

### Local Testing
```bash
# Test with local database
MigrationService.exe --auth-db "Host=localhost;Database=LegalTest;Username=test;Password=test"
```

### Docker Testing
```bash
# Start test PostgreSQL
docker run --name test-postgres -e POSTGRES_PASSWORD=test -p 5432:5432 -d postgres:13

# Run migration against test database
MigrationService.exe --auth-db "Host=localhost;Database=postgres;Username=postgres;Password=test"
```

## 📦 Dependencies

### Core Dependencies
- **CommandLineParser** (2.9.1) - Command line argument parsing
- **dbup-postgresql** (5.0.40) - Database migration framework
- **DistributedLock.Postgres** (1.2.1) - Distributed locking
- **Microsoft.EntityFrameworkCore.Design** (9.0.8) - EF Core design tools
- **Npgsql.EntityFrameworkCore.PostgreSQL** (9.0.4) - PostgreSQL provider

### Project References
- **Legal.Application.Admin** - Admin module for context access
- **Legal.Service.Infrastructure** - Base infrastructure services
- **Legal.Service.Repository** - Data access layer
- **Legal.Shared.SharedModel** - Shared models

## 🚀 Deployment

### Production Deployment

1. **Build Release**
   ```bash
   dotnet build --configuration Release
   dotnet publish --configuration Release --output ./publish
   ```
2. **Deploy Files**
   - Copy published files to target server
   - Ensure connection strings are configured
   - Test connectivity to database
3. **Execute Migration**
   ```bash
   ./MigrationService --update-database "y" --context-number "-1"
   ```

### CI/CD Integration

```yaml
# Example Azure DevOps pipeline step
- task: DotNetCoreCLI@2
  displayName: 'Run Database Migrations'
  inputs:
    command: 'run'
    projects: 'OtherServices/MigrationService/MigrationService.csproj'
    arguments: '--update-database "y" --context-number "-1"'
  env:
    ConnectionStrings__Postgres: $(DatabaseConnectionString)
```

## 🧯 Troubleshooting

### Common Issues

#### Connection Failures
```
Error: Unable to connect to database
Solution: Verify connection string and database availability
```

#### Migration Conflicts
```
Error: Migration already applied
Solution: Check migration history table for conflicts
```

#### Permission Errors
```
Error: Permission denied for schema
Solution: Ensure database user has appropriate permissions
```

### Debugging
- **Verbose Logging** - Enable detailed logging in configuration
- **Connection Testing** - Test database connectivity separately
- **Manual Verification** - Check migration history in database

## 🔮 Future Enhancements

- **Migration Rollback** - Ability to rollback specific migrations
- **Parallel Execution** - Multi-threaded migration processing
- **Health Checks** - Database health verification
- **Migration Validation** - Pre-migration validation checks
- **Notification System** - Email/Slack notifications for migration results

## 📚 References

- [Entity Framework Core Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [DbUp Documentation](https://dbup.readthedocs.io/)
- [CommandLineParser](https://github.com/commandlineparser/commandline)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

---
## License
MIT License © Taufiq Abdur Rahman
You may not use this codebase without permission. For Evolution purposes only.

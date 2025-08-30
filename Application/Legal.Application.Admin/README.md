# Legal.Application.Admin

## Overview
Administrative (Admin) application layer for the Legal system. Implements CQRS style command / query handler discovery, EF Core (PostgreSQL, snake_case), AutoMapper mapping, and domain services for Users & Contracts.

## Architecture
Implements Clean Architecture application layer responsibilities:
- Command Handlers: state-changing operations (User, Contract, Auth)
- Query Handlers: (extensible – add for read scenarios)
- EF Core DbContext: AdminDatabaseContext with naming conventions & migration history table
- Mapping (AutoMapper) via centralized MappingConfig
- Domain Services (e.g., RegistrationService, Contract service abstraction)
- Parameter / Response models (shared project) consumed by handlers
- Dependency injection extension for module bootstrap

## Project Structure (excerpt)
```
Legal.Application.Admin/
  CommandHandlers/
    Auth/
      RegistrationCommandHandler.cs
      LogInCommandHandler.cs
      ChangePasswordCommandHandler.cs
    User/
      EditUserCommandHandler.cs
      DeleteUserCommandHandler.cs
    Contract/
      SaveContractCommandHandler.cs
      DeleteContractCommandHandler.cs
  Dtos/
    UserDto.cs
    ContractDto.cs
  Infrastructure/
    IContractService.cs (and implementation in Service layer)
  EntityConfigurations/
    UserEntityTypeConfiguration.cs
    AuditableEntityTypeConfiguration.cs
  AdminDatabaseContext.cs
  MappingConfig.cs
  DependencyInjection.cs
  Migrations/ (generated EF migrations)
  DataSeeder/public.user.json
```

## Key Features
### User & Auth
- Registration (email uniqueness, password hashing)
- Login (JWT token issuance)
- Change password
- Edit / delete user

### Contracts
- Save (create / update) contract (SaveContractCommandHandler)
- Delete contract (DeleteContractCommandHandler)
(Additional queries to list / get contracts can be added using query handlers.)

### Shared Concerns
- Automatic handler registration & caching via RequestHandler
- AutoMapper mapping profiles created fluently
- Background JSON seeding (see root guide) – copies initial users

## AdminDatabaseContext
Configured in DependencyInjection:
```csharp
services.AddDbContextFactory<AdminDatabaseContext>(options =>
    options.UseNpgsql(conn, x => x.MigrationsHistoryTable("__EFMigrationsHistory", ModuleName.ADMIN.ToString()))
           .UseSnakeCaseNamingConvention());
```
Includes entity configurations (User, auditable base). Migration history scoped per module.

## Dependency Injection Extension
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddAdminApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddDbContextFactory<AdminDatabaseContext>(options =>
        {
            var cs = configuration.GetConnectionString(ConnectionStringsConstants.Postgres);
            options.UseNpgsql(cs, x => x.MigrationsHistoryTable("__EFMigrationsHistory", ModuleName.ADMIN.ToString()))
                   .UseSnakeCaseNamingConvention();
        });
        services.RegisterTypes(Assembly.GetExecutingAssembly(), typeof(IBaseModel), typeof(IEntityTypeConfiguration<>));
        services.AddLogging();
        var mapper = MappingConfig.RegisterMappings();
        services.AddScoped(_ => mapper);
        var handlers = services.RegisterHandlers(Assembly.GetExecutingAssembly(), typeof(ACommandHandler<,>), typeof(AQueryHandler<,>));
        RequestHandler.SetRequestHandlers(ModuleName.ADMIN, handlers);
        return services;
    }
}
```
(Use `AddAdminApplicationLayer` in API Program.cs.)

## Mapping Configuration
`MappingConfig` sets up projection paths:
- User -> UserResponseModel / UserDto
- RegistrationParameterModel -> UserDto
- ContractParameterModel <-> ContractDto -> Contract -> ContractResponseModel
Call `MappingConfig.RegisterMappings()` during DI extension.

## Command Handlers
Examples:
- `RegistrationCommandHandler` – create user & token
- `LogInCommandHandler` – authenticate & token
- `ChangePasswordCommandHandler` – update password
- `EditUserCommandHandler` – modify profile/name
- `DeleteUserCommandHandler` – soft delete (implementation may adapt repository logic)
- `SaveContractCommandHandler` – upsert contract
- `DeleteContractCommandHandler` – delete contract

Pattern: Each handler derives from `ACommandHandler<TParameter, TResponse>` and overrides
```csharp
public override Task<TResponse> Execute(TParameter parameter, CancellationToken ct)
```
Attributes like `[TokenAuthorize]` enforce auth where required.

## Adding a New Handler
1. Create parameter model in Shared project if not existing.
2. Implement handler deriving from `ACommandHandler` or `AQueryHandler`.
3. Ensure mapping if model/entity translation is needed.
4. Build – DI extension auto-discovers & registers; RequestHandler caches it.

## Usage (API Invocation)
API layer posts a JSON body specifying the handler name (convention) & parameter payload.
Example (Login):
```json
{
  "name": "LogInCommandHandler",
  "data": { "email": "admin@example.com", "password": "P@ssw0rd" }
}
```
Save Contract:
```json
{
  "name": "SaveContractCommandHandler",
  "data": { "id": null, "name": "NDA", "author": "Admin", "description": "Mutual NDA" }
}
```

## Migrations
Generate migrations from this project (context lives here):
```bash
dotnet ef migrations add Init --project Application/Legal.Application.Admin --startup-project Api/Legal.Api.WebApi

dotnet ef database update --project Application/Legal.Application.Admin --startup-project Api/Legal.Api.WebApi
```
(Startup project ensures proper design-time services.)

## Data Seeding
`DataSeeder/public.user.json` is copied to output (Always). Combined with API seeding infrastructure you can insert initial admin user(s).

## Security
- Password hashing (implementation in Service layer)
- JWT issuance (Token service – outside this project but invoked by handlers)
- `[TokenAuthorize]` attribute for protected handlers

## Performance & Reliability
- DbContext factory for scoped creation in handlers
- Snake_case naming reduces friction with PostgreSQL
- AutoMapper pre-initialization reduces runtime config cost
- MemoryCache available for read optimization (future queries)

## Testing (Roadmap)
- Unit tests: handler Execute logic (mock IRepository / services)
- Integration: in-memory or Testcontainers PostgreSQL applying migrations
- Mapping tests: assert configuration validity via `mapper.ConfigurationProvider.AssertConfigurationIsValid()`

## Future Enhancements
- Query handlers for contract listing & filtering
- Audit logging & soft delete standardization
- Role / permission granularity & claims enrichment
- Pagination & caching strategies
- Validation layer per parameter (FluentValidation integration in shared models)

## References
- EF Core Docs
- AutoMapper Docs
- Clean Architecture / CQRS guidance


---
## License
MIT License © Taufiq Abdur Rahman
You may not use this codebase without permission. For Evolution purposes only.

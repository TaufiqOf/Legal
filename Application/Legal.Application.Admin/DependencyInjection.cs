using Legal.Service.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Legal.Service.Helper.ApplicationHelper;

namespace Legal.Application.Admin;

public static class DependencyInjection
{
    public static IServiceCollection AddExpenseApplicationLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddDbContextFactory<AdminDatabaseContext>(options =>
        {
            var connectionString = configuration.GetConnectionString(ConnectionStringsConstants.Postgres);

            options
                .UseNpgsql(connectionString, x => x
                    .MigrationsHistoryTable("__EFMigrationsHistory", (string)ModuleName.ADMIN.ToString()))
                .UseSnakeCaseNamingConvention();
        });

        services.RegisterTypes(Assembly.GetExecutingAssembly(), typeof(IBaseModel), typeof(IEntityTypeConfiguration<>));

        services.AddLogging();

        var mapper = MappingConfig.RegisterMappings();
        services.AddScoped(_ => mapper);

        var handlers = services.RegisterHandlers(
            Assembly.GetExecutingAssembly(),
            typeof(ACommandHandler<,>),
            typeof(AQueryHandler<,>));

        foreach (var handler in handlers) Console.WriteLine($"Registering handler: {handler.Name}");

        RequestHandler.SetRequestHandlers(ModuleName.ADMIN, handlers);

        return services;
    }
}
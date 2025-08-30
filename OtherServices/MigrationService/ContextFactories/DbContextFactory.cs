using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics; // added for warning suppression

namespace Legal.MigrationService.ContextFactories;

public class DbContextFactory<T> : IDesignTimeDbContextFactory<T> where T : ADatabaseContext
{
    private readonly Configuration _configuration;

    public DbContextFactory() : base()
    {
        _configuration = new Configuration(null);
    }

    public DbContextFactory(Configuration configuration) : base()
    {
        _configuration = configuration;
    }

    public T CreateDbContext(string[] args)
    {
        var connectionString = _configuration.ConString;

        // Temporary instance to get schema name
        T? tempContext = (T)Activator.CreateInstance(typeof(T));
        var schema = tempContext.SchemaName();

        var optionsBuilder = new DbContextOptionsBuilder<T>()
            .UseNpgsql(connectionString, npgSqlOptions =>
            {
                npgSqlOptions.MigrationsAssembly(typeof(T).Assembly.FullName);
                npgSqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schema);
            })
            // Suppress pending model changes warning (EF would otherwise throw in your scenario)
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

        return (T)Activator.CreateInstance(typeof(T), optionsBuilder.Options)!;
    }
}
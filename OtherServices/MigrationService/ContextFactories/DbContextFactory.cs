using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Legel.MigrationService.ContextFactories;

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
        var builder = new DbContextOptionsBuilder<T>();

        T? aDatabaseContext = (T)Activator.CreateInstance(typeof(T));

        var options = new DbContextOptionsBuilder<T>()
            .UseNpgsql(connectionString, npgSqlOptions =>
            {
                npgSqlOptions.MigrationsAssembly(typeof(T).Assembly.FullName);
                npgSqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", aDatabaseContext.SchemaName());
            }).Options;

        T? fDatabaseContext = (T)Activator.CreateInstance(typeof(T), options);

        return fDatabaseContext;
    }
}
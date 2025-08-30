using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Legal.MigrationService;

public class Migrator<T> where T : ADatabaseContext
{
    private readonly Configuration _configuration;
    private readonly ContextFactories.DbContextFactory<T> _commonContextFactory;

    public Migrator(Configuration configuration, ContextFactories.DbContextFactory<T> dbContextFactory)
    {
        _configuration = configuration;
        _commonContextFactory = dbContextFactory;
    }

    public void MigrateDb()
    {
        using (var cc = _commonContextFactory.CreateDbContext([]))
        {
            Console.WriteLine($"Migrating: Server={cc?.Database.GetDbConnection().Database};");
            if (cc.Database.GetPendingMigrations().Any())
            {
                cc.Database.Migrate();
                Console.WriteLine($"Migrate Successfull...");
            }
            else
            {
                Console.WriteLine($"Nothing to migrate...");
            }
        }
    }
}
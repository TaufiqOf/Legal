using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Legal.MigrationService;

public class Configuration
{
    private IConfigurationRoot _configuration { get; set; }

    private readonly Options? _options;

    public string ConString { get; private set; }

    public Configuration(Options? options)
    {
        _options = options;
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: true)
            .AddEnvironmentVariables() // allow Docker env overrides (e.g. ConnectionStrings__Postgres)
            .Build();

        ConString = ResolveConnectionString();
    }

    private string ResolveConnectionString()
    {
        // 1. command line override
        if (!string.IsNullOrWhiteSpace(_options?.StreamDataConnection))
        {
            return _options.StreamDataConnection.Trim();
        }

        // 2. environment variable (standard ASP.NET style)
        var envConn = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres");
        if (!string.IsNullOrWhiteSpace(envConn))
        {
            return envConn.Trim();
        }

        // 3. legacy/custom env fallbacks
        var customEnv = Environment.GetEnvironmentVariable("DATA_DB")
                         ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
                         ?? Environment.GetEnvironmentVariable("POSTGRES_CONN");
        if (!string.IsNullOrWhiteSpace(customEnv))
        {
            return customEnv.Trim();
        }

        // 4. appsettings.json
        var appsettingsConn = _configuration.GetConnectionString("Postgres");
        if (!string.IsNullOrWhiteSpace(appsettingsConn) && appsettingsConn != "*")
        {
            return appsettingsConn.Trim();
        }

        return string.Empty;
    }
}
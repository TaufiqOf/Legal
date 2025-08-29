using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Legel.MigrationService
{
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
                .AddJsonFile("appsettings.json")
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: true)
                .Build();

            ConString = GetConStrings() ?? string.Empty;
        }

        private string GetConStrings()
        {
            var connectionString = _configuration.GetConnectionString("Postgres") ?? string.Empty;

            if (_options != null)
            {
                if (!string.IsNullOrEmpty(_options.StreamDataConnection))
                    connectionString = _options.StreamDataConnection;
            }

            return connectionString ?? string.Empty;
        }
    }
}
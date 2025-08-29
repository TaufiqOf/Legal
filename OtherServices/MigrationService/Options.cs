using CommandLine;

namespace Legel.MigrationService
{
    public class Options
    {
        [Option("data-db", Required = false, HelpText = "Set application database connection string.")]
        public string StreamDataConnection { get; set; } = default!;

        [Option("update-database", Required = false, HelpText = "Set application database connection string.")]
        public string UpdateDatabase { get; set; } = default!;

        [Option("context-number", Required = false, HelpText = "Set dbatabase context to run the migration no.")]
        public string ContextNumber { get; set; } = default!;
    }
}
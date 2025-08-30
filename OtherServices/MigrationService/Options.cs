using CommandLine;

namespace Legal.MigrationService;

public class Options
{
    [Option("data-db", Required = false, HelpText = "Set application database connection string.")]
    public string StreamDataConnection { get; set; } = default!;

    [Option("update-database", Required = false, HelpText = "Apply migrations automatically when set to 'y'.")]
    public string UpdateDatabase { get; set; } = "n"; // default no auto update

    [Option("context-number", Required = false, HelpText = "Database context selection (-1 = all, comma/range supported). Defaults to -1.")]
    public string ContextNumber { get; set; } = "-1"; // default all contexts so non-interactive when update-database=y
}
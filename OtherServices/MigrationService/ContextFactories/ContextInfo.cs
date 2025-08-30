using Microsoft.EntityFrameworkCore;

namespace Legal.MigrationService.ContextFactories;

public class ContextInfo
{
    public string Name { get; set; }

    public DbContext DbContext { get; set; }
}
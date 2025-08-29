using Microsoft.EntityFrameworkCore;

namespace Legel.MigrationService.ContextFactories
{
    public class ContextInfo
    {
        public string Name { get; set; }

        public DbContext DbContext { get; set; }
    }
}
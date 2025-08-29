using Microsoft.EntityFrameworkCore;

namespace Legal.Service.Infrastructure.Model;

public abstract class ADatabaseContext : DbContext
{
    public ADatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public ADatabaseContext() : base()
    {
    }

    public abstract string SchemaName();
}
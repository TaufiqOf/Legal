using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Legal.Service.Repository;

public class PostgresRepository<T, TDbContext> : APostgresRepository<T, TDbContext>, IRepository<T, TDbContext>
    where T : BaseModel
    where TDbContext : DbContext
{
    public PostgresRepository(IDbContextFactory<TDbContext> factory, TDbContext context) : base(factory, context)
    {
    }
}
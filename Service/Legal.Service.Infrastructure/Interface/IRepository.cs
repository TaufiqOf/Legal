using Microsoft.EntityFrameworkCore;

namespace Legal.Service.Infrastructure.Interface;

public interface IRepository<T, TDbContext> : IBaseRepository<T, TDbContext>
    where T : IBaseModel
    where TDbContext : DbContext
{
}
using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Legal.Service.Infrastructure.Interface;

public interface IDomainRepository<T, TDbContext> : IBaseRepository<T, TDbContext>
    where T : DomainBaseModel
    where TDbContext : DbContext
{
    Task Delete(T entity, bool softDelete = false, CancellationToken cancellationToken = default);

    Task Delete(IEnumerable<T> entities, bool softDelete = false, CancellationToken cancellationToken = default);

    Task<T?> Get(string id, bool isDeleted = false, CancellationToken cancellationToken = default);

    Task<IList<T>> GetAll(bool isDeleted = false, CancellationToken cancellationToken = default);

    Task<(IList<T> Items, int Total)> GetPagedData(int pageNumber, int pageSize, bool isDeleted = false, CancellationToken cancellationToken = default);

    Task<(IList<T> Items, int Total)> GetPagedData(IQueryable<T> query, int pageNumber, int pageSize, bool isDeleted = false, CancellationToken cancellationToken = default);

    IQueryable<T> GetQueryable(bool isDeleted);
}
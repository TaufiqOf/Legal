using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Legal.Service.Infrastructure.Interface;

public interface IBaseRepository<T, TDbContext>
    where T : IBaseModel
    where TDbContext : DbContext
{
    Task<T?> Get(string id, CancellationToken cancellationToken = default);

    Task<IList<T>> GetAll(CancellationToken cancellationToken = default);

    Task<T> Add(T entity, CancellationToken cancellationToken = default);

    Task Update(T entity);

    Task Delete(T entity);

    Task Delete(IQueryable<T> entities);

    Task Delete(IEnumerable<T> entities);

    IQueryable<T> GetQueryable();

    Task<bool> Exists(T item);

    void Detach(T entity);

    Task Add(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task Update(IEnumerable<T> entities);

    Task<T> Execute(ASqlModel sqlModel);

    Task<IEnumerable<T>> ExecuteList(ASqlModel sqlModel, params object[] parameters);

    Task<string?> ExecuteQuery(ASqlModel sqlModel);

    Task<X?> ExecuteQuery<X>(
        ASqlModel sqlModel,
        params object[] parameters);

    void DetachAll();

    Task Commit(CancellationToken cancellationToken = default);
}
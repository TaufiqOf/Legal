using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Legal.Service.Repository;

public class DomainPostgresRepository<T, TDbContext> : APostgresRepository<T, TDbContext>, IDomainRepository<T, TDbContext>
    where T : DomainBaseModel
    where TDbContext : DbContext
{
    private readonly IAccessToken _accessToken;

    public DomainPostgresRepository(
        IDbContextFactory<TDbContext> factory,
        TDbContext context,
        IAccessToken accessToken) : base(factory, context)
    {
        _accessToken = accessToken;
    }

    public async Task<(IList<T> Items, int Total)> GetPagedData(int pageNumber, int pageSize, bool isDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = DBSet.AsQueryable().Where(q => q.IsDeleted == isDeleted);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<(IList<T> Items, int Total)> GetPagedData(IQueryable<T> query, int pageNumber, int pageSize, bool isDeleted = false, CancellationToken cancellationToken = default)
    {
        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public IQueryable<T> GetQueryable(bool isDeleted)
    {
        return DBSet.AsQueryable().Where(q => q.IsDeleted == isDeleted).AsQueryable();
    }

    public override async Task<T> Add(T entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedBy = _accessToken?.UserId ?? entity.CreatedBy;
        return await base.Add(entity, cancellationToken);
    }

    public override async Task Add(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            entity.CreatedBy = _accessToken?.UserId ?? entity.CreatedBy;
        }

        await base.Add(entities, cancellationToken);
    }

    public override async Task Update(T entity)
    {
        entity.LastModifiedBy = _accessToken?.UserId ?? entity.LastModifiedBy;

        await base.Update(entity);
    }

    public override async Task Update(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.LastModifiedBy = _accessToken?.UserId ?? entity.LastModifiedBy;
        }

        await base.Update(entities);
    }

    public virtual async Task Delete(T entity, bool softDelete = false, CancellationToken cancellationToken = default)
    {
        if (!softDelete)
        {
            DBSet.Remove(entity);
        }
        else
        {
            entity.IsDeleted = true;
            await base.Update(entity);
        }

        await Task.CompletedTask;
    }

    public async Task Delete(IEnumerable<T> entities, bool softDelete = false, CancellationToken cancellationToken = default)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        foreach (var e in entities)
        {
            await Delete(e, softDelete, cancellationToken);
        }
    }

    public virtual async Task<T?> Get(string id, bool isDeleted = false, CancellationToken cancellationToken = default)
    {
        return await DBSet.AsQueryable().FirstOrDefaultAsync(q => q.Id == id && q.IsDeleted == isDeleted, cancellationToken);
    }

    public virtual async Task<IList<T>> GetAll(bool isDeleted = false, CancellationToken cancellationToken = default)
    {
        return await DBSet.AsQueryable().Where(q => q.IsDeleted == isDeleted).AsNoTracking().ToListAsync(cancellationToken);
    }
}
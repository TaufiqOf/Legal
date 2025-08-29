using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using System.Data.Common;

namespace Legal.Service.Repository;

public abstract class APostgresRepository<T, TDbContext> : IBaseRepository<T, TDbContext> where T : BaseModel where TDbContext : DbContext
{
    public DbSet<T> DBSet { get; set; }

    public APostgresRepository(IDbContextFactory<TDbContext> factory, TDbContext context)
    {
        Context = context;
        DBSet = context.Set<T>();
        Factory = factory;
    }

    public TDbContext Context { get; set; }

    public IDbContextFactory<TDbContext> Factory { get; }

    public virtual async Task<T> Add(T entity, CancellationToken cancellationToken = default)
    {
        var newEntity = await DBSet.AddAsync(entity, cancellationToken);
        return newEntity.Entity;
    }

    public virtual async Task Add(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await Context.AddRangeAsync(entities, cancellationToken);
    }

    public virtual async Task Update(T entity)
    {
        entity.LastModifiedTime = DateTimeOffset.UtcNow;
        DBSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task Update(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.LastModifiedTime = DateTimeOffset.UtcNow;
            Context.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        await Task.CompletedTask;
    }

    public virtual void Detach(T entity)
    {
        DBSet.Entry(entity).State = EntityState.Detached;
    }

    public virtual void DetachAll()
    {
        Context.ChangeTracker.Entries<T>();
        Context.ChangeTracker.Clear();
    }

    public virtual async Task Delete(T entity)
    {
        DBSet.Remove(entity);
        await Task.CompletedTask;
    }

    public virtual async Task<T?> Get(string id, CancellationToken cancellationToken = default)
    {
        return await DBSet.AsQueryable().FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public virtual async Task<IList<T>> GetAll(CancellationToken cancellationToken = default)
    {
        return await DBSet.AsQueryable().AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual IQueryable<T> GetQueryable()
    {
        return DBSet.AsQueryable();
    }

    public virtual async Task Delete(IQueryable<T> entities)
    {
        DBSet.RemoveRange(entities);
        await Task.CompletedTask;
    }

    public virtual async Task Delete(IEnumerable<T> entities)
    {
        DBSet.RemoveRange(entities);
        await Task.CompletedTask;
    }

    public virtual async Task Delete(List<T> entities)
    {
        DBSet.RemoveRange(entities);
        await Task.CompletedTask;
    }

    public virtual Task<bool> Exists(T item)
    {
        return DBSet.AsQueryable().AnyAsync(q => q.Id == item.Id);
    }

    public virtual async Task Commit(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<T?> Execute(ASqlModel sqlModel)
    {
        ArgumentException.ThrowIfNullOrEmpty(sqlModel?.Sql);
        var query = sqlModel.Sql;
        var result = await DBSet.FromSqlRaw(query).FirstOrDefaultAsync();
        return result;
    }

    public virtual async Task<IEnumerable<T>> ExecuteList(
        ASqlModel sqlModel,
        params object[] parameters)
    {
        ArgumentException.ThrowIfNullOrEmpty(sqlModel?.Sql);

        var query = sqlModel.Sql;
        var dbParamters = CreateParameters(parameters);
        var result = await DBSet
            .FromSqlRaw(query, dbParamters)
            .ToListAsync();

        return result;
    }

    public virtual async Task<string?> ExecuteQuery(ASqlModel sqlModel)
    {
        ArgumentException.ThrowIfNullOrEmpty(sqlModel?.Sql);

        var connection = Context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using (var command = connection.CreateCommand())
        {
            command.CommandText = sqlModel.Sql;
            var result = await command.ExecuteScalarAsync();
            return result?.ToString();
        }
    }

    public virtual async Task<X?> ExecuteQuery<X>(
        ASqlModel sqlModel,
        params object[] parameters)
    {
        ArgumentException.ThrowIfNullOrEmpty(sqlModel?.Sql);

        DbConnection connection = Context.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using var command = connection.CreateCommand();

        command.CommandText = sqlModel.Sql;
        command.Parameters.AddRange(CreateParameters(parameters));

        List<Dictionary<string, object>> results = [];
        using DbDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Dictionary<string, object> row = [];

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);

                // Convert DB-style names to PascalCase
                string normalized = ToPascalCase(columnName);
                row[normalized] = reader.GetValue(i);
            }

            results.Add(row);
        }

        string resultsAsJson = JsonConvert.SerializeObject(results, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        return JsonConvert.DeserializeObject<X>(resultsAsJson, settings);
    }

    private static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // handle snake_case → PascalCase
        var parts = input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Select(p => char.ToUpperInvariant(p[0]) + p.Substring(1)));
    }

    private NpgsqlParameter[] CreateParameters(params object[] parameters)
    {
        if (parameters == null || parameters.Length == 0)
        {
            return [];
        }

        List<NpgsqlParameter> dbParameters = [];

        foreach (var param in parameters)
        {
            foreach (var property in param.GetType().GetProperties())
            {
                dbParameters.Add(new NpgsqlParameter(
                    parameterName: $"@{property.Name}",
                    value: property.GetValue(param)));
            }
        }

        return dbParameters.ToArray();
    }
}
using Legal.Application.Admin;
using Legal.Shared.SharedModel.SqlModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using System.Reflection;

namespace Legal.Api.WebApi.DataSeeding;

internal class DataSeederService(
        IConfiguration configuration,
        IDbContextFactory<AdminDatabaseContext> dbContextFactory,
        ILogger<DataSeederService> logger
    ) : BackgroundService
{
    private AdminDatabaseContext _context = dbContextFactory.CreateDbContext();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        List<InitializationOptions>? initializationOptions = configuration
            .GetSection("InitializationOptions")
            .Get<List<InitializationOptions>>();
        if (initializationOptions is null)
        {
            return;
        }

        foreach (InitializationOptions option in initializationOptions)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string jsonText = await File.ReadAllTextAsync(Path.Combine(path, option.FilePath));
            List<Dictionary<string, object>>? data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonText);
            if (data is null)
            {
                continue;
            }

            string tableName = Path.GetFileNameWithoutExtension(option.FilePath);

            List<object> jsonIds = data
                .Where(d => d.ContainsKey("id"))
                .Select(d => d["id"]!)
                .ToList();

            HashSet<string> existingIds = await GetExistingIdsAsync(tableName, jsonIds, stoppingToken);

            foreach (Dictionary<string, object> record in data)
            {
                try
                {
                    if (!record.ContainsKey("id"))
                    {
                        continue;
                    }

                    object id = record["id"]!;
                    if (!existingIds.Contains(id))
                    {
                        await InsertRecordAsync(tableName, record);
                    }
                    else if (option.DoUpdate)
                    {
                        await UpdateRecordAsync(tableName, record);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }
            }
        }
    }

    private async Task InsertRecordAsync(
        string tableName,
        Dictionary<string, object> record)
    {
        string columns = string.Join(", ", record.Keys.ToList());
        string values = string.Join(", ", record.Keys.Select(k => $"@{k}"));
        NpgsqlParameter[] parameters = GetParameters(record);

        InsertSqlModel insertSqlModel = new(tableName, columns, values);
        await _context.Database.ExecuteSqlRawAsync(insertSqlModel.Sql, parameters);
    }

    private async Task UpdateRecordAsync(
        string tableName,
        Dictionary<string, object> record)
    {
        IEnumerable<string> updateAssignments = record
            .Where(kv => !kv.Key.Equals("id", StringComparison.OrdinalIgnoreCase))
            .Select(kv => $"{kv.Key} = @{kv.Key}");
        string updateClause = string.Join(", ", updateAssignments);
        NpgsqlParameter[] parameters = GetParameters(record);

        UpdateSqlModel updateSqlModel = new(tableName, record["id"], updateClause);
        await _context.Database.ExecuteSqlRawAsync(updateSqlModel.Sql, parameters);
    }

    private async Task<HashSet<string>> GetExistingIdsAsync(string tableName, List<object> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return new HashSet<string>();
        }

        var paramNames = ids.Select((_, index) => $"@id{index}").ToList();
        var parameters = ids
            .Select((id, index) => new NpgsqlParameter($"@id{index}", id ?? DBNull.Value))
            .ToArray();

        var sqlModel = new GetIdsInDBSqlModel(tableName, paramNames);
        var result = await _context.Database.SqlQueryRaw<string>(sqlModel.Sql, parameters)
            .ToListAsync(cancellationToken);

        return result.ToHashSet();
    }

    private NpgsqlParameter[] GetParameters(Dictionary<string, object> record)
    {
        return [.. record.Keys.Select(column => new NpgsqlParameter($"@{column}", record[column] ?? DBNull.Value))];
    }
}
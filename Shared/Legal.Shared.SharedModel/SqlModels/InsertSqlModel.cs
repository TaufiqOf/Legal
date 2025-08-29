using Legal.Service.Infrastructure.Model;

namespace Legal.Shared.SharedModel.SqlModels;

public sealed class InsertSqlModel(
    string tableName,
    string columns,
    string values) : ASqlModel
{
    public override string Sql { get; set; } = $"""
        INSERT INTO {tableName} ({columns}, create_time)
        VALUES ({values}, now())
        ON CONFLICT(id) DO NOTHING
        ;
    """;
}
using Legal.Service.Infrastructure.Model;

namespace Legal.Shared.SharedModel.SqlModels;

public sealed class UpdateSqlModel(
    string tableName,
    object idValue,
    string updateClause) : ASqlModel
{
    public override string Sql { get; set; } = $"""
        UPDATE {tableName}
        SET {updateClause}, last_modified_time = now()
        WHERE id = '{idValue}';
    """;
}
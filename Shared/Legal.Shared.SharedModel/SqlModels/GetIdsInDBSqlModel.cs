using Legal.Service.Infrastructure.Model;

namespace Legal.Shared.SharedModel.SqlModels;

public sealed class GetIdsInDBSqlModel(string tableName, IEnumerable<string> parameterNames) : ASqlModel
{
    public override string Sql { get; set; } = $"""
        SELECT id FROM {tableName} WHERE id IN ({string.Join(", ", parameterNames)})
    """;
}
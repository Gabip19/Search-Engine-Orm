using System.Text;
using OrmLibrary.Execution;

namespace OrmLibrary.SqlServer;

public class SqlServerQueryGenerator : ISqlQueryGenerator
{
    public string GenerateQuery<TEntity>(QueryContext<TEntity> queryContext) where TEntity : class, new()
    {
        var tableProperties = OrmContext.CurrentEntityModels.EntitiesMappings[typeof(TEntity)];
        
        var sqlBuilder = new StringBuilder();
        sqlBuilder.Append("SELECT ");

        if (queryContext.SelectedColumns.Any())
        {
            sqlBuilder.Append(string.Join(", ", queryContext.SelectedColumns));
        }
        else
        {
            sqlBuilder.Append(string.Join(", ", tableProperties.Columns.Select(c => c.Name)));
        }

        sqlBuilder.Append($" FROM {tableProperties.Name}");

        if (queryContext.WhereConditions.Any())
        {
            sqlBuilder.Append(" WHERE ");
            var whereClauses = queryContext.WhereConditions.Select(c => $"{c.PropertyName} {c.Operation} {FormatValue(c.Value)}");
            sqlBuilder.Append(string.Join(" AND ", whereClauses));
        }

        if (queryContext.OrderByColumns.Any())
        {
            sqlBuilder.Append(" ORDER BY ");
            var orderByClauses = queryContext.OrderByColumns.Select(o => $"{o.PropertyName} {(o.IsAscending ? "ASC" : "DESC")}");
            sqlBuilder.Append(string.Join(", ", orderByClauses));
        }

        if (queryContext.Skip.HasValue)
        {
            sqlBuilder.Append($" OFFSET {queryContext.Skip.Value} ROWS");
        }

        if (queryContext.Take.HasValue)
        {
            sqlBuilder.Append($" FETCH NEXT {queryContext.Take.Value} ROWS ONLY");
        }

        return sqlBuilder.ToString();
    }

    private static string FormatValue(dynamic value)
    {
        if (value is string || value is DateTime)
        {
            return $"'{value}'";
        }
        return value.ToString();
    }
}

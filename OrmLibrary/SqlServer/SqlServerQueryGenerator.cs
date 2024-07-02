using System.Text;
using OrmLibrary.Execution;
using OrmLibrary.Execution.Query;
using OrmLibrary.Mappings;

namespace OrmLibrary.SqlServer;

public class SqlServerQueryGenerator : ISqlQueryGenerator
{
    public QuerySqlDto GenerateQuery<TEntity>(QueryContext<TEntity> queryContext) where TEntity : class, new()
    {
        var entityType = typeof(TEntity);
        var mappings = OrmContext.CurrentEntityModels.EntitiesMappings;
        var tableProps = mappings[entityType];
        IList<KeyValuePair<string, object>> queryParameters = new List<KeyValuePair<string, object>>();
        
        var sb = new StringBuilder();
        
        //// SELECT
        
        sb.Append("SELECT ");
        if (queryContext.AggregateMethod == AggregateMethod.COUNT)
        {
            sb.Append("COUNT(*) ");
        }
        else
        {
            BuildSelectQuery(queryContext, sb, tableProps, entityType, mappings);
        }

        sb.Append('\n');
        
        //// FROM
        
        BuildFromQuery(queryContext, sb, tableProps);
        sb.Append('\n');
        
        //// WHERE
        
        if (queryContext.WhereConditions.Any())
        {
            sb.Append("WHERE ");
            var whereQuery = GenerateWhereQueryString(queryContext.WhereConditions, tableProps);

            queryParameters = whereQuery.Parameters;
            sb.Append(whereQuery.Sql);
        }

        sb.Append('\n');

        //// ORDER BY
        
        if (queryContext.OrderByColumns.Any())
        {
            sb.Append("ORDER BY ");
            var orderByClauses = queryContext.OrderByColumns.Select(o => $"{ColumnName(o.PropertyName, tableProps)} {(o.IsAscending ? "ASC" : "DESC")}");
            sb.Append(string.Join(", ", orderByClauses));
        }

        sb.Append('\n');

        // Skip and Take for pagination
        BuildSkipSql(queryContext, sb);

        return new QuerySqlDto
        {
            Parameters = queryParameters,
            Sql = sb.ToString()
        };
    }

    private static void BuildSkipSql<TEntity>(QueryContext<TEntity> queryContext, StringBuilder sb) where TEntity : class, new()
    {
        if (queryContext.Skip.HasValue || queryContext.Take.HasValue)
        {
            if (!queryContext.OrderByColumns.Any())
            {
                throw new InvalidOperationException("ORDER BY clause is required when using SKIP or TAKE.");
            }

            if (queryContext.Skip.HasValue)
            {
                sb.Append($"OFFSET {queryContext.Skip.Value} ROWS");
            }

            if (queryContext.Take.HasValue)
            {
                sb.Append($" FETCH NEXT {queryContext.Take.Value} ROWS ONLY");
            }
        }
    }

    private void BuildFromQuery<TEntity>(QueryContext<TEntity> queryContext, StringBuilder sb, TableProperties tableProps)
        where TEntity : class, new()
    {
        sb.Append($"FROM {tableProps.Name}");

        foreach (var propertyToLoadName in queryContext.ReferencePropertiesToLoad)
        {
            var innerJoin = BuildInnerJoinQueryString(tableProps, propertyToLoadName);
            sb.Append(innerJoin);
        }
    }

    private void BuildSelectQuery<TEntity>(QueryContext<TEntity> queryContext, StringBuilder sb, TableProperties tableProps,
        Type entityType, MappedEntitiesCollection mappings) where TEntity : class, new()
    {
        sb.Append(BuildSelectQueryString(queryContext, tableProps));

        foreach (var propertyToLoadName in queryContext.ReferencePropertiesToLoad)
        {
            sb.Append(", ");
            
            var property = entityType.GetProperty(propertyToLoadName) ??
                           throw new ArgumentException($"{propertyToLoadName} is an invalid referenced property name to load");
            var selectColumns = BuildFullPropertiesSelectQuery(mappings[property.PropertyType]);
            
            sb.Append(selectColumns);
        }
    }

    private QuerySqlDto GenerateWhereQueryString(IReadOnlyList<WhereConditionDetails> whereConditions, TableProperties tableProps)
    {
        var sb = new StringBuilder();
        var currentGroupLevel = 1;
        var parameterIndex = 0;
        var parameters = new List<KeyValuePair<string, object>>();

        for (var i = 0; i < whereConditions.Count; i++)
        {
            var condition = whereConditions[i];

            while (condition.GroupLevel > currentGroupLevel)
            {
                sb.Append('(');
                currentGroupLevel++;
            }

            while (condition.GroupLevel < currentGroupLevel)
            {
                sb.Append(')');
                currentGroupLevel--;
            }

            CreateWhereClause(parameters, tableProps, condition, sb, ref parameterIndex);
        }

        while (currentGroupLevel > 1)
        {
            sb.Append(')');
            currentGroupLevel--;
        }

        return new QuerySqlDto
        {
            Parameters = parameters,
            Sql = sb.ToString()
        };
    }

    private static void CreateWhereClause(ICollection<KeyValuePair<string, object>> parameters, TableProperties tableProps,
        WhereConditionDetails condition, StringBuilder sb, ref int parameterIndex)
    {
        if (!string.IsNullOrEmpty(condition.LogicalOperator))
        {
            sb.Append($" {condition.LogicalOperator} ");
            return;
        }

        if (condition.Value is null)
        {
            switch (condition.Operation)
            {
                case "=":
                    sb.Append($"{ColumnName(condition.PropertyName, tableProps)} IS NULL");
                    break;
                case "!=":
                    sb.Append($"{ColumnName(condition.PropertyName, tableProps)} IS NOT NULL");
                    break;
            }

            return;
        }

        switch (condition.Operation)
        {
            case "NULL_OR_EMPTY":
            case "NULL_OR_WHITESPACE":
                sb.Append($"ISNULL({ColumnName(condition.PropertyName, tableProps)})");
                return;
            case "IN":
                sb.Append($"{ColumnName(condition.PropertyName, tableProps)} IN {condition.Value}");
                return;
        }

        var parameterName = $"@param{parameterIndex++}";
        parameters.Add(new KeyValuePair<string, object>(parameterName, condition.Value));
        
        sb.Append($"{ColumnName(condition.PropertyName, tableProps)} {condition.Operation} {parameterName}");
    }

    private string BuildInnerJoinQueryString(TableProperties tableProps, string fkPropertyName)
    {
        var sb = new StringBuilder(" INNER JOIN ");
        
        var fkGroup = tableProps.GetAssociatedForeignKeyGroup(fkPropertyName) ??
                      throw new ArgumentException($"No foreign key is associated for the {fkPropertyName}.");

        sb.Append($"{fkGroup.ReferencedTableName} ON ");
        sb.Append(string.Join(" AND ",
            fkGroup.KeyPairs.Select(pair => $"{ColumnName(pair.MainColumn)} = {ColumnName(pair.ReferencedColumn)}")));

        return sb.ToString();
    }

    private string BuildFullPropertiesSelectQuery(TableProperties tableProps)
    {
        var sb = new StringBuilder();

        sb.Append(string.Join(", ", tableProps.Columns.Select(properties => ColumnName(properties))));
        return sb.ToString();
    }

    private string BuildSelectQueryString<TEntity>(QueryContext<TEntity> queryContext, TableProperties tableProps)  where TEntity : class, new()
    {
        var sb = new StringBuilder();
        
        if (queryContext.AggregateMethod is not null)
        {
            sb.Append($"{queryContext.AggregateMethod}({ColumnName(queryContext.AggregatedColumn, tableProps)})");
            return sb.ToString();
        }

        if (queryContext.SelectedColumns.Count > 0)
        {
            sb.Append(string.Join(", ", queryContext.SelectedColumns.Select(s => ColumnName(s, tableProps))));
        }
        else
        {
            sb.Append(BuildFullPropertiesSelectQuery(tableProps));
        }

        return sb.ToString();
    }

    private static string ColumnName(string propName, TableProperties tableProps)
    {
        return ColumnName(Column(propName, tableProps), tableProps);
    }

    private static string ColumnName(ColumnProperties columnProp, TableProperties? tableProp = null)
    {
        return $"{tableProp?.Name ?? columnProp.TableName}.{columnProp.Name}";
    }

    private static ColumnProperties Column(string propName, TableProperties tableProps)
    {
        return tableProps.GetColumnInfo(propName) ?? throw new ArgumentException(
            $"Property {propName} does not have a corresponding mapped column in the {tableProps.Name} table");
    }
}

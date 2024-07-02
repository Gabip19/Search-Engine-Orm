using System.Data;
using System.Reflection;
using OrmLibrary.Execution.Query;
using OrmLibrary.Mappings;

namespace OrmLibrary.Execution;

public class DbQueryExecutor
{
    public QueryExecutionResult<TEntity> ExecuteQuery<TEntity>(QuerySqlDto sqlQuery, IDbConnection connection, IDbTransaction transaction) where TEntity : class, new()
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = sqlQuery.Sql;
            AddQueryParameters(command, sqlQuery.Parameters);
            command.Transaction = transaction;

            using (var reader = command.ExecuteReader())
            {
                if (sqlQuery.IsScalar)
                {
                    return new QueryExecutionResult<TEntity>
                    {
                        ScalarResult = ExtractScalarQueryResult(reader)
                    };
                }

                return new QueryExecutionResult<TEntity>
                {
                    Results = ExtractListQueryResult<TEntity>(reader, sqlQuery.SelectedProperties, sqlQuery.PropertiesToLoad)
                };
            }
        }
    }

    private IList<TEntity> ExtractListQueryResult<TEntity>(IDataReader reader, HashSet<string> selectedProperties, HashSet<string> propertiesToLoad) where TEntity : class, new()
    {
        var results = new List<TEntity>();
        while (reader.Read())
        {
            var entity = new TEntity();
            var tableProps = GetTableProperties<TEntity>();
            
            foreach (var property in typeof(TEntity).GetProperties())
            {
                if (selectedProperties.Count > 0 && !selectedProperties.Contains(property.Name)) continue;
                if (propertiesToLoad.Contains(property.Name))
                {
                    var loadedValue = LoadForeignKeyProperty(property, reader);
                    property.SetValue(entity, loadedValue);
                    continue;
                }
                
                var columnProps = tableProps.GetColumnInfoByProperty(property.Name);
                if (columnProps is null)
                {
                    continue;
                }
                
                var value = ConvertValue(reader[columnProps.Name], columnProps.LanguageNativeType);
                property.SetValue(entity, value);
            }
            results.Add(entity);
        }

        return results;
    }

    private object? ConvertValue(object? value, Type type)
    {
        if (value == DBNull.Value || value is null)
        {
            return null;
        }
        if (type == typeof(char))
        {
            return ((string)value)[0];
        }
        if (type == typeof(Guid))
        {
            return Guid.Parse(value.ToString()!);
        }
        if (type == typeof(DateTime))
        {
            return DateTime.Parse(value.ToString()!);
        }

        return value;
    }
    
    private object? LoadForeignKeyProperty(PropertyInfo property, IDataReader reader)
    {
        var entity = Activator.CreateInstance(property.PropertyType);
        var tableProps = GetTableProperties(property.PropertyType);

        foreach (var prop in property.PropertyType.GetProperties())
        {
            var columnProps = tableProps.GetColumnInfoByProperty(prop.Name);
            if (columnProps is null)
            {
                continue;
            }
                
            var value = ConvertValue(reader[columnProps.Name], columnProps.LanguageNativeType);
            prop.SetValue(entity, value);
        }

        return entity;
    }

    private static object? ExtractScalarQueryResult(IDataReader reader)
    {
        return reader.Read() ? reader[0] : null;
    }

    private void AddQueryParameters(IDbCommand command, IEnumerable<KeyValuePair<string, object>> parameters)
    {
        foreach (var parameter in parameters)
        {
            var dbParameter = command.CreateParameter();
            dbParameter.ParameterName = parameter.Key;
            dbParameter.Value = parameter.Value;
            command.Parameters.Add(dbParameter);
        }
    }
    
    private TableProperties GetTableProperties<TEntity>()
    {
        return OrmContext.CurrentEntityModels.EntitiesMappings[typeof(TEntity)];
    }
    
    private TableProperties GetTableProperties(Type type)
    {
        return OrmContext.CurrentEntityModels.EntitiesMappings[type];
    }
}
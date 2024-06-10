using System.Data.SqlClient;
using OrmLibrary.Mappings;

namespace OrmLibrary.Execution;

public class QueryExecutor
{
    private readonly string _connectionString;

    public QueryExecutor(string connectionString)
    {
        _connectionString = connectionString;
    }

    public QueryExecutionResult<TEntity> ExecuteQuery<TEntity>(string sql) where TEntity : class, new()
    {
        var results = new List<TEntity>();

        using var connection = new SqlConnection(_connectionString);
        
        connection.Open();
        using (var command = new SqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var entity = new TEntity();
                    var tableProps = GetTableProperties<TEntity>();
                    foreach (var property in typeof(TEntity).GetProperties())
                    {
                        var columnName = tableProps.GetColumnInfoByProperty(property.Name)?.Name ?? property.Name;
                        var value = reader[columnName];
                        property.SetValue(entity, value == DBNull.Value ? null : value);
                    }
                    results.Add(entity);
                }
            }
        }

        return new QueryExecutionResult<TEntity>
        {
            Results = results
        };
    }

    private TableProperties GetTableProperties<TEntity>()
    {
        return OrmContext.CurrentEntityModels.EntitiesMappings[typeof(TEntity)];
    }
}
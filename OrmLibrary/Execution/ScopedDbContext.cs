using OrmLibrary.SqlServer;

namespace OrmLibrary.Execution;

public class ScopedDbContext
{
    private readonly ISqlQueryGenerator _sqlGenerator;

    private readonly IConnectionProvider _connectionProvider =
        new SqlServerConnectionProvider(OrmContext.ConnectionString);

    private readonly QueryExecutor _queryExecutor = new();
    
    public ScopedDbContext(ISqlQueryGenerator sqlGenerator)
    {
        _sqlGenerator = sqlGenerator;
    }
    
    public DbTable<TEntity> Entity<TEntity>() where TEntity : class, new()
    {
        return new DbTable<TEntity>(this);
    }

    public QueryExecutionResult<TEntity> Execute<TEntity>(QueryContext<TEntity> queryContext) where TEntity : class, new()
    {
        var sqlQuery = _sqlGenerator.GenerateQuery(queryContext);

        using var connection = _connectionProvider.CreateConnection();
        connection.Open();
        
        return _queryExecutor.ExecuteQuery<TEntity>(sqlQuery, connection);
    }

    public int ExecuteSqlCommand(string sql)
    {
        using var connection = _connectionProvider.CreateConnection();
        connection.Open();

        return 0;
    }
}
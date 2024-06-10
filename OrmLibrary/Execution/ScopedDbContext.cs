namespace OrmLibrary.Execution;

public class ScopedDbContext
{
    private readonly ISqlQueryGenerator _sqlGenerator;
    
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
        var executor = new QueryExecutor(OrmContext.ConnectionString);
        var sqlQuery = _sqlGenerator.GenerateQuery(queryContext);
        return executor.ExecuteQuery<TEntity>(sqlQuery);
    }
}
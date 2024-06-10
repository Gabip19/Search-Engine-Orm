namespace OrmLibrary.Execution;

public class ScopedDbContext
{
    public DbTable<TEntity> Entity<TEntity>() where TEntity : class, new()
    {
        return new DbTable<TEntity>(this);
    }

    public QueryExecutionResult<TEntity> Execute<TEntity>(QueryContext<TEntity> queryContext) where TEntity : class, new()
    {
        // get db connection, pass to the querytranslator, then pass to the query executor which also extracts the data,
        // maps back to objects and returns the values inside the QueryExecutionResult
        return new QueryExecutionResult<TEntity>();
    }
}
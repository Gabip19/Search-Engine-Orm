namespace OrmLibrary.Execution;

public class DbTable<TEntity> where TEntity : class, new()
{
    private readonly ScopedDbContext _dbContext;
    private QueryBuilder<TEntity> _currentQueryBuilder;

    public DbTable(ScopedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public QueryBuilder<TEntity> Query()
    {
        _currentQueryBuilder = new QueryBuilder<TEntity>(this);
        return _currentQueryBuilder;
    }

    public QueryExecutionResult<TEntity> ExecuteQuery(QueryContext<TEntity> queryContext)
    {
        return _dbContext.Execute(queryContext);
    }
}
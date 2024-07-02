namespace OrmLibrary.Execution;

public class QueryExecutionResult<TEntity> where TEntity : class, new()
{
    public IList<TEntity>? Results { get; set; }
    public object? ScalarResult { get; set; }
}

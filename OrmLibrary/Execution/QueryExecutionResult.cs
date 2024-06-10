namespace OrmLibrary.Execution;

public class QueryExecutionResult<TEntity> where TEntity : class, new()
{
    public List<TEntity> Results { get; set; } = new();
}

namespace OrmLibrary.Execution;

public interface ISqlQueryGenerator
{
    string GenerateQuery<TEntity>(QueryContext<TEntity> queryContext) where TEntity : class, new();
}
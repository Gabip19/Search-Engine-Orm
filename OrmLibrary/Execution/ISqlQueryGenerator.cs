using OrmLibrary.Execution.Query;

namespace OrmLibrary.Execution;

public interface ISqlQueryGenerator
{
    QuerySqlDto GenerateQuery<TEntity>(QueryContext<TEntity> queryContext) where TEntity : class, new();
}
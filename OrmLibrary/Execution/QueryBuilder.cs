using System.Linq.Expressions;
using OrmLibrary.Execution.Parsers;
using OrmLibrary.Extensions;

namespace OrmLibrary.Execution;

public class QueryBuilder<TEntity> where TEntity : class, new()
{
    private readonly QueryContext<TEntity> _queryContext = new();
    private readonly DbTable<TEntity> _dbTable;
    
    public QueryBuilder(DbTable<TEntity> dbTable)
    {
        _dbTable = dbTable;
    }

    public QueryBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        var visitor = new WhereExpressionVisitor();
        visitor.Visit(predicate);
        
        _queryContext.WhereConditions.AddRange(visitor.Comparisons);
        return this;
    }

    public QueryBuilder<TEntity> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        var visitor = new SelectExpressionVisitor();
        visitor.Visit(selector);

        _queryContext.SelectedColumns.AddRange(visitor.SelectedColumns);
        return this;
    }

    private QueryBuilder<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool isAscending)
    {
        var propertyName = ExtensionsHelper.GetPropertyName(keySelector);
        _queryContext.OrderByColumns.Add((propertyName, isAscending));
        return this;
    }

    public QueryBuilder<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector)
    {
        return OrderBy(keySelector, true);
    }

    public QueryBuilder<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector)
    {
        return OrderBy(keySelector, false);
    }

    public QueryBuilder<TEntity> Skip(int count)
    {
        _queryContext.Skip = count;
        return this;
    }

    public QueryBuilder<TEntity> Take(int count)
    {
        _queryContext.Take = count;
        return this;
    }

    public void Execute()
    {
        _dbTable.ExecuteQuery(_queryContext);
    }
}

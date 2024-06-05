using System.Linq.Expressions;
using OrmLibrary.Extensions;

namespace OrmLibrary;

public class QueryBuilder<TEntity> where TEntity : class, new()
{
    private readonly QueryContext<TEntity> _queryContext = new();

    public QueryBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        _queryContext.WhereExpressions.Add(predicate);
        return this;
    }

    public QueryBuilder<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        _queryContext.SelectedColumns.AddRange(ExtractColumns(selector));
        return new QueryBuilder<TResult>(_connection, _transaction, _logger, _serviceProvider);
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
}

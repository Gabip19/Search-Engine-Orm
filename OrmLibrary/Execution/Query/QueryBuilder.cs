using System.Linq.Expressions;
using OrmLibrary.Execution.Parsers;

namespace OrmLibrary.Execution.Query;

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
        foreach (var condition in _queryContext.WhereConditions)
        {
            condition.GroupLevel += 1;
        }
        
        var visitor = new WhereExpressionVisitor();
        visitor.Visit(predicate);

        if (visitor.Conditions.Count == 1)
        {
            visitor.Conditions[0].GroupLevel += 1;
        }

        if (_queryContext.WhereConditions.Count > 0)
        {
            _queryContext.WhereConditions.Add(new WhereConditionDetails
            {
                LogicalOperator = " AND ",
                GroupLevel = 1
            });
            
            foreach (var condition in visitor.Conditions)
            {
                condition.GroupLevel += 1;
            }
        }
        
        _queryContext.WhereConditions.AddRange(visitor.Conditions);
        return this;
    }

    public QueryBuilder<TEntity> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        var visitor = new SelectExpressionVisitor();
        visitor.Visit(selector);

        foreach (var selectedColumn in visitor.SelectedColumns)
        {
            _queryContext.SelectedColumns.Add(selectedColumn);
        }
        return this;
    }

    public QueryBuilder<TEntity> Load<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        var propertyName = GetPropertyName(selector);
        _queryContext.ReferencePropertiesToLoad.Add(propertyName);
        
        return this;
    }

    private QueryBuilder<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool isAscending)
    {
        var propertyName = GetPropertyName(keySelector);
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
        if (_queryContext.Take is not null)
        {
            throw new InvalidOperationException(
                $"The method {nameof(Take)} can not be used if the method was previously invoked or if the {nameof(First)} method was invoked.");
        }
        _queryContext.Take = count;
        return this;
    }

    public TEntity? First()
    {
        if (_queryContext.Take is not null)
        {
            throw new InvalidOperationException(
                $"The method {nameof(First)} can not be used if the method {nameof(Take)} was already invoked.");
        }

        _queryContext.Take = 1;
        
        return ExecuteQuery().Results?.FirstOrDefault();
    }
    
    public TResult Max<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        if (_queryContext.AggregateMethod is not null)
        {
            throw new InvalidOperationException(
                $"The method {nameof(Max)} can not be used if any of the aggregate methods was already invoked.");
        }

        AddAggregationOperation(selector, AggregateMethod.MAX);

        return ExecuteScalar<TResult>();
    }
    
    public TResult Min<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        if (_queryContext.AggregateMethod is not null)
        {
            throw new InvalidOperationException(
                $"The method {nameof(Min)} can not be used if any of the aggregate methods was already invoked.");
        }

        AddAggregationOperation(selector, AggregateMethod.MIN);

        return ExecuteScalar<TResult>();
    }
    
    public TResult Average<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        if (_queryContext.AggregateMethod is not null)
        {
            throw new InvalidOperationException(
                $"The method {nameof(Average)} can not be used if any of the aggregate methods was already invoked.");
        }

        AddAggregationOperation(selector, AggregateMethod.AVG);
        
        return ExecuteScalar<TResult>();
    }
    
    public TResult Sum<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        if (_queryContext.AggregateMethod is not null)
        {
            throw new InvalidOperationException(
                $"The method {nameof(Sum)} can not be used if any of the aggregate methods was already invoked.");
        }

        AddAggregationOperation(selector, AggregateMethod.SUM);
        
        return ExecuteScalar<TResult>();
    }
    
    private void AddAggregationOperation<TResult>(Expression<Func<TEntity, TResult>> selector, AggregateMethod method)
    {
        _queryContext.AggregateMethod = method;
        _queryContext.AggregatedColumn = GetPropertyName(selector);
    }
    
    public int Count()
    {
        if (_queryContext.AggregateMethod is not null)
        {
            throw new InvalidOperationException(
                $"The method {nameof(Count)} can not be used if any of the aggregate methods was already invoked.");
        }

        _queryContext.AggregateMethod = AggregateMethod.COUNT;
        
        return ExecuteScalar<int>();
    }
    
    private static string GetPropertyName<TKey>(Expression<Func<TEntity, TKey>> expr)
    {
        return expr.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression unaryMember } => unaryMember.Member.Name,
            _ => throw new ArgumentException("Expression is not a member access", nameof(expr))
        };
    }
    
    public IEnumerable<TEntity> Execute()
    {
        return ExecuteQuery().Results ?? new List<TEntity>();
    }
    
    private TResult ExecuteScalar<TResult>()
    {
        var scalarResult = ExecuteQuery().ScalarResult ?? default;

        if (scalarResult is null)
        {
            throw new InvalidOperationException("The query did not return any results.");
        }

        try
        {
            return (TResult)Convert.ChangeType(scalarResult, typeof(TResult));
        }
        catch (InvalidCastException)
        {
            throw new InvalidOperationException($"Failed to convert the result to type {typeof(TResult).Name}.");
        }
    }
    
    private QueryExecutionResult<TEntity> ExecuteQuery()
    {
        return _dbTable.ExecuteQuery(_queryContext);
    }
}

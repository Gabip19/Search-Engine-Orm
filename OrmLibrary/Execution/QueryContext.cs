using System.Linq.Expressions;

namespace OrmLibrary;

public class QueryContext<TEntity> where TEntity : class, new()
{
    public List<Expression<Func<TEntity, bool>>> WhereExpressions { get; } = new();
    public List<string> SelectedColumns { get; } = new();
    public List<(string PropertyName, bool IsAscending)> OrderByColumns { get; } = new();
    public int? Skip { get; set; }
    public int? Take { get; set; }
}

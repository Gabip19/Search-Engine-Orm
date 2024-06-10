using OrmLibrary.Execution.Query;

namespace OrmLibrary.Execution;

public class QueryContext<TEntity> where TEntity : class, new()
{
    public List<WhereConditionDetails> WhereConditions { get; } = new();
    public List<string> SelectedColumns { get; } = new();
    public List<(string PropertyName, bool IsAscending)> OrderByColumns { get; } = new();
    public int? Skip { get; set; }
    public int? Take { get; set; }
}

using OrmLibrary.Execution.Query;

namespace OrmLibrary.Execution;

public class QueryContext<TEntity> where TEntity : class, new()
{
    public List<WhereConditionDetails> WhereConditions { get; } = new();
    public HashSet<string> SelectedColumns { get; } = new();
    public HashSet<string> ReferencePropertiesToLoad { get; } = new();
    public HashSet<(string PropertyName, bool IsAscending)> OrderByColumns { get; } = new();
    public int? Skip { get; set; }
    public int? Take { get; set; }
    public AggregateMethod? AggregateMethod { get; set; }
    public string AggregatedColumn { get; set; }
}

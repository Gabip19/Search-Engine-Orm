namespace OrmLibrary.Execution.Query;

public class QuerySqlDto
{
    public string Sql { get; set; }
    public IList<KeyValuePair<string, object>> Parameters { get; set; }
    public bool IsScalar { get; set; }
    public HashSet<string> SelectedProperties { get; set; }
    public HashSet<string> PropertiesToLoad { get; set; }
}
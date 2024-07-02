namespace OrmLibrary.Execution.Query;

public class QuerySqlDto
{
    public string Sql { get; set; }
    public IList<KeyValuePair<string, object>> Parameters { get; set; }
}
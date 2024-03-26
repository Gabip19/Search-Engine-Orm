namespace OrmLibrary;

public class TableProperties
{
    public string Name { get; set; }
    public IList<ColumnProperties> PrimaryKeys { get; }

    private readonly IDictionary<string, ColumnProperties> _columnProperties;

    public TableProperties()
    {
        PrimaryKeys = new List<ColumnProperties>();
        _columnProperties = new Dictionary<string, ColumnProperties>();
    }

    public void RegisterColumn(ColumnProperties column)
    {
        _columnProperties[column.Name] = column;

        if (column.IsPrimaryKeyColumn)
        {
            PrimaryKeys.Add(column);
        }
    }

    public ColumnProperties? GetColumnInfo(string columnName)
    {
        return _columnProperties[columnName];
    }
}
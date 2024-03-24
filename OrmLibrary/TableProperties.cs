namespace OrmLibrary;

public class TableProperties
{
    public string Name { get; set; }
    public ColumnProperties PrimaryKey { get; set; }

    private IDictionary<string, ColumnProperties> _columnProperties;

    public TableProperties()
    {
        _columnProperties = new Dictionary<string, ColumnProperties>();
    }

    public void RegisterColumn(ColumnProperties column)
    {
        _columnProperties[column.Name] = column;
    }

    public ColumnProperties? GetColumnInfo(string columnName)
    {
        return _columnProperties[columnName];
    }
}
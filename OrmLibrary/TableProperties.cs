using System.Collections.ObjectModel;

namespace OrmLibrary;

public class TableProperties
{
    public string Name { get; set; }
    public Type AssociatedType { get; set; }
    public IList<ColumnProperties> PrimaryKeys { get; }

    public IReadOnlyCollection<ColumnProperties> Columns =>
        new ReadOnlyCollection<ColumnProperties>(_columnProperties.Values.ToList());
    
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
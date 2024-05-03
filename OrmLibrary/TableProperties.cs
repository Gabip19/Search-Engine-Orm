using System.Collections.ObjectModel;

namespace OrmLibrary;

public class TableProperties
{
    public string Name { get; set; }
    public Type AssociatedType { get; set; }
    public IList<ColumnProperties> PrimaryKeys { get; }

    public IReadOnlyList<ColumnProperties> Columns =>
        new ReadOnlyCollection<ColumnProperties>(_columnPropertiesByName.Values.ToList());
    
    private readonly IDictionary<string, ColumnProperties> _columnPropertiesByName;
    private readonly IDictionary<string, ColumnProperties> _columnPropertiesByPropertyName;

    public TableProperties()
    {
        PrimaryKeys = new List<ColumnProperties>();
        _columnPropertiesByName = new Dictionary<string, ColumnProperties>();
        _columnPropertiesByPropertyName = new Dictionary<string, ColumnProperties>();
    }

    public void RegisterColumn(ColumnProperties column)
    {
        _columnPropertiesByName[column.Name] = column;
        _columnPropertiesByPropertyName[column.PropertyName] = column;

        if (column.IsPrimaryKeyColumn)
        {
            PrimaryKeys.Add(column);
        }
    }

    public ColumnProperties? GetColumnInfo(string columnName)
    {
        return _columnPropertiesByName.TryGetValue(columnName, out var value) ? value : null;
    }

    public ColumnProperties? GetColumnInfoByProperty(string propertyName)
    {
        return _columnPropertiesByPropertyName.TryGetValue(propertyName, out var value) ? value : null;
    }
    
    public bool TryGetColumnInfo(string columnName, out ColumnProperties? value)
    {
        value = GetColumnInfo(columnName);
        return value != null;
    }
    
    public bool TryGetColumnInfoByProperty(string propertyName, out ColumnProperties? value)
    {
        value = GetColumnInfoByProperty(propertyName);
        return value != null;
    }
}
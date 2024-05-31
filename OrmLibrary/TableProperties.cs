using System.Collections.ObjectModel;
using OrmLibrary.Constraints;

namespace OrmLibrary;

public class TableProperties
{
    public string Name { get; set; }
    public Type AssociatedType { get; set; }
    public IList<ColumnProperties> PrimaryKeys { get; }
    public IList<ITableConstraint> Constraints { get; }
    
    public IList<ForeignKeyGroup> ForeignKeys => 
        _foreignKeys.Values.ToList();
    
    public IReadOnlyList<ColumnProperties> Columns =>
        new ReadOnlyCollection<ColumnProperties>(_columnPropertiesByName.Values.ToList());
    
    private readonly IDictionary<string, ColumnProperties> _columnPropertiesByName;
    private readonly IDictionary<string, ColumnProperties> _columnPropertiesByPropertyName;
    private readonly IDictionary<string, ForeignKeyGroup> _foreignKeys;

    public TableProperties()
    {
        PrimaryKeys = new List<ColumnProperties>();
        Constraints = new List<ITableConstraint>();
        _columnPropertiesByName = new Dictionary<string, ColumnProperties>();
        _columnPropertiesByPropertyName = new Dictionary<string, ColumnProperties>();
        _foreignKeys = new Dictionary<string, ForeignKeyGroup>();
    }

    public void RegisterColumn(ColumnProperties column)
    {
        column.Table = this;

        if (!_columnPropertiesByName.TryAdd(column.Name, column))
            throw new ArgumentException("A column with the same name already exists.", column.Name);

        if (column.IsPrimaryKeyColumn)
        {
            PrimaryKeys.Add(column);
        }
        
        if (column.IsForeignKeyColumn)
        {
            _foreignKeys.TryAdd(
                column.ForeignKeyGroup!.AssociatedProperty.Name,
                column.ForeignKeyGroup
            );
            // TODO: maybe have a ForeignKeyPair be composed of two ColumnProperties instead of column names
        }
        else
        {
            _columnPropertiesByPropertyName[column.PropertyName!] = column;
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

    public void AddConstraint(ITableConstraint constraint)
    {
        Constraints.Add(constraint);
    }
}
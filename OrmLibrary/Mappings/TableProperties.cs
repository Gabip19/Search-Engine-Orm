using System.Collections.ObjectModel;
using OrmLibrary.Constraints;

namespace OrmLibrary.Mappings;

public class TableProperties
{
    public string Name { get; set; }
    public Type? AssociatedType { get; set; }
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

    // TODO: maybe create a ColumnCollection class to store the Columns and add define the methods there for separation of concerns
    public void RegisterColumn(ColumnProperties column)
    {
        column.TableName = Name;
        
        if (!_columnPropertiesByName.TryAdd(column.Name, column))
            throw new ArgumentException("A column with the same name already exists.", column.Name);

        if (column.IsPrimaryKeyColumn)
        {
            PrimaryKeys.Add(column);
        }
        
        if (column.IsForeignKeyColumn)
        {
            if (column.ForeignKeyGroup is not null)
            {
                RegisterForeignKeyGroup(column.ForeignKeyGroup);
            }
        }
        else
        {
            _columnPropertiesByPropertyName[column.PropertyName!] = column;
        }

        if (column.IsUnique)
        {
            RegisterUniqueConstraint(column);
        }
    }

    public void RegisterForeignKeyGroup(ForeignKeyGroup fkGroup)
    {
        if (!_foreignKeys.TryAdd(fkGroup.AssociatedPropertyName, fkGroup)) return;
        
        foreach (var fkGroupKeyPair in fkGroup.KeyPairs)
        {
            fkGroupKeyPair.MainColumn.ForeignKeyGroup = fkGroup;
        }
            
        Constraints.Add(new ForeignKeyConstraint
        {
            Name = $"FK_{Name}_{fkGroup.ReferencedTableName}_{fkGroup.AssociatedPropertyName}",
            ForeignKeyGroup = fkGroup,
            TableName = Name,
            ReferencedTableName = fkGroup.ReferencedTableName
        });
    }
    
    private void RegisterUniqueConstraint(ColumnProperties column)
    {
        Constraints.Add(new UniqueConstraint
        {
            Name = $"UQ_{column.Name}",
            ColumnName = column.Name
        });
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
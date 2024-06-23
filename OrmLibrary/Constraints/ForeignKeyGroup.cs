using System.Reflection;
using OrmLibrary.Extensions;

namespace OrmLibrary.Constraints;

public class ForeignKeyGroup
{
    private PropertyInfo? _associatedProperty;
    
    public string AssociatedPropertyName { get; set; }
    public PropertyInfo? AssociatedProperty
    {
        get => _associatedProperty;
        set
        {
            _associatedProperty = value;
            if (value is not null)
            {
                AssociatedPropertyName = value.Name;
            }
        }
    }
    public string ReferencedTableName { get; set; }
    public string ColumnsNamesPrefix { get; set; }
    public IList<ForeignKeyPair> KeyPairs { get; set; } = new List<ForeignKeyPair>();
}
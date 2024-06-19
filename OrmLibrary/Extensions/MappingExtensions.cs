using OrmLibrary.Constraints;
using OrmLibrary.Serialization;

namespace OrmLibrary.Extensions;

public static class MappingExtensions
{
    public static ForeignKeyGroupDto MapToDto(this ForeignKeyGroup foreignKeyGroup)
    {
        var columns = new List<string>();
        var referencedColumns = new List<string>();

        var columnsPairs = foreignKeyGroup.KeyPairs;
        
        foreach (var pair in columnsPairs)
        {
            columns.Add(pair.MainColumn.Name);
            referencedColumns.Add(pair.ReferencedColumn.Name);
        }

        return new ForeignKeyGroupDto
        {
            AssociatedPropertyName = foreignKeyGroup.AssociatedProperty.Name,
            Columns = columns,
            ReferencedColumns = referencedColumns
        };
    }
}
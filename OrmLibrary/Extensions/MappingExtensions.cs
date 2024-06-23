using OrmLibrary.Constraints;
using OrmLibrary.Mappings;
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
            AssociatedPropertyName = foreignKeyGroup.AssociatedPropertyName,
            ReferencedTableName = foreignKeyGroup.ReferencedTableName,
            Columns = columns,
            ReferencedColumns = referencedColumns,
            ColumnsNamesPrefix = foreignKeyGroup.ColumnsNamesPrefix
        };
    }
    
    public static Dictionary<string, TableProperties> MapToTableProperties(IDictionary<string, TablePropertiesDto> dtoMappings)
    {
        var tableMappings = dtoMappings.ToDictionary(pair => pair.Key, pair => pair.Value.UnlinkedTableProperties);

        foreach (var tablePropsDto in dtoMappings.Values)
        {
            foreach (var foreignKeyGroupDto in tablePropsDto.KeyGroupsDtos)
            {
                var foreignKeyGroup = foreignKeyGroupDto.MapToForeignGroup(tableMappings);
                tablePropsDto.UnlinkedTableProperties.RegisterForeignKeyGroup(foreignKeyGroup);
            }
        }

        return tableMappings;
    }
    
    public static ForeignKeyGroup MapToForeignGroup(this ForeignKeyGroupDto dto, Dictionary<string, TableProperties> tableMappings)
    {
        var associatedProperty = tableMappings[dto.AssociatedTableName].AssociatedType?.GetProperty(dto.AssociatedPropertyName);
        
        var group = new ForeignKeyGroup
        {
            ReferencedTableName = dto.ReferencedTableName,
            AssociatedProperty = associatedProperty,
            AssociatedPropertyName = dto.AssociatedPropertyName,
            ColumnsNamesPrefix = dto.ColumnsNamesPrefix
        };

        var associatedTableMapping = tableMappings[dto.AssociatedTableName];
        var referencedTableMapping = tableMappings[dto.ReferencedTableName];
        
        for (var i = 0; i < dto.Columns.Count; i++)
        {
            group.KeyPairs.Add(new ForeignKeyPair
            {
                MainColumn = associatedTableMapping.GetColumnInfo(dto.Columns[i])!,
                ReferencedColumn = referencedTableMapping.GetColumnInfo(dto.ReferencedColumns[i])!
            });
        }

        return group;
    }
}
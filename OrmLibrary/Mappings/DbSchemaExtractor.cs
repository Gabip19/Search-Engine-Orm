using System.Reflection;
using OrmLibrary.Attributes.Relational;
using OrmLibrary.Constraints;
using OrmLibrary.Converters;
using OrmLibrary.Extensions;
using OrmLibrary.SqlServer;

namespace OrmLibrary.Mappings;

public static class DbSchemaExtractor
{
    private static readonly ISqlTypeConverter Converter = new SqlServerTypeConverter();

    public static ICollection<TableProperties> ExtractTablesProperties(IEnumerable<Type> entitiesTypes)
    {
        return entitiesTypes.Select(ExtractTableProperties).ToList();
    }
    
    public static TableProperties ExtractTableProperties(Type entityType)
    {
        var tableProps = new TableProperties
        {
            Name = ExtensionsHelper.GetTableName(entityType),
            AssociatedType = entityType
        };

        Console.WriteLine($"Extracting table props for: {entityType.Name}");
        
        foreach (var primaryKeyColumn in GetPrimaryKeysColumns(entityType))
        {
            tableProps.RegisterColumn(primaryKeyColumn);
        }
        
        foreach (var property in entityType.GetProperties().Where(info => !info.IsPrimaryKeyProperty()))
        {
            MapProperty(property, tableProps);
        }

        return tableProps;
    }

    private static void MapProperty(PropertyInfo property, TableProperties tableProps)
    {
        if (property.IsManyToOneProperty())
        {
            RegisterPropertyMappings(tableProps, RegisterForeignKeyColumns(property));
        }
        else if (property.IsOneToManyProperty())
        {
            
        }
        else if (property.IsManyToManyProperty())
        {
            
        }
        else if (property.IsOneToOneProperty(out var oneToOneAttribute))
        {
            if (string.IsNullOrEmpty(oneToOneAttribute!.MappedByColumnName))
            {
                RegisterPropertyMappings(tableProps, RegisterForeignKeyColumns(property));
            }
        }
        else if (property.IsForeignKeyProperty())
        {
            RegisterPropertyMapping(tableProps, RegisterForeignKeyColumn(property));
        }
        else
        {
            RegisterPropertyMapping(tableProps, ExtractColumnProperties(property));
        }
    }

    private static void RegisterPropertyMapping(TableProperties tableProps, ColumnProperties propertyMapping)
    {
        tableProps.RegisterColumn(propertyMapping);
    }
    
    private static void RegisterPropertyMappings(TableProperties tableProps, IEnumerable<ColumnProperties> propertyMappings)
    {
        foreach (var propertyMapping in propertyMappings)
        {
            tableProps.RegisterColumn(propertyMapping);
        }
    }

    private static ColumnProperties ExtractColumnProperties(PropertyInfo property)
    {
        var columnBaseType = property.GetBaseType();
        
        return new ColumnProperties
        {
            Name = ExtensionsHelper.GetColumnName(property),
            TableName = ExtensionsHelper.GetTableName(property.DeclaringType!),
            PropertyName = property.Name,
            IsPrimaryKeyColumn = property.IsPrimaryKeyProperty(),
            IsNullable = property.IsNullable(),
            LanguageNativeType = columnBaseType,
            SqlColumnType = Converter.ConvertToSqlType(columnBaseType),
            MaxLength = property.GetMaxLength(),
            IsUnique = property.HasUniqueValue()
        };
    }

    private static readonly IDictionary<Type, List<ColumnProperties>> PrimaryKeysCache = new Dictionary<Type, List<ColumnProperties>>();

    private static IList<ColumnProperties> GetPrimaryKeysColumns(Type entityType)
    {
        if (PrimaryKeysCache.TryGetValue(entityType, out var primaryKeys))
        {
            return primaryKeys;
        }

        primaryKeys = new List<ColumnProperties>();
        
        foreach (var property in ExtensionsHelper.GetPrimaryKeyProperties(entityType))
        {
            if (property.PropertyType.IsMappedEntityType())
            {
                primaryKeys.AddRange(MapToForeignKeyColumns(GetPrimaryKeysColumns(property.PropertyType), property));
            }
            else
            {
                primaryKeys.Add(ExtractColumnProperties(property));
            }
        }

        PrimaryKeysCache.Add(entityType, primaryKeys);
        
        return primaryKeys;
    }

    private static ColumnProperties RegisterForeignKeyColumn(PropertyInfo property)
    {
        var foreignKeyAttribute = property.GetCustomAttribute<ForeignKeyAttribute>()!;
        var referencedProperty = foreignKeyAttribute.ReferencedType.GetProperty(foreignKeyAttribute.ReferencedColumnName)!;
        var mappedColumn = ExtractColumnProperties(property);
        var referencedColumn = ExtractColumnProperties(referencedProperty);
        
        mappedColumn.IsForeignKeyColumn = true;
        mappedColumn.ForeignKeyGroup = new ForeignKeyGroup
        {
            AssociatedProperty = property,
            ColumnsNamesPrefix = ExtensionsHelper.GetColumnsNamesPrefix(property),
            ReferencedTableName = referencedColumn.TableName,
            KeyPairs = new List<ForeignKeyPair>
            {
                new()
                {
                    MainColumn = mappedColumn,
                    ReferencedColumn = referencedColumn
                }
            }
        };
        
        return mappedColumn;
    }
    
    private static IList<ColumnProperties> RegisterForeignKeyColumns(PropertyInfo foreignKeyProp)
    {
        var referencedPrimaryKeyColumns = GetPrimaryKeysColumns(foreignKeyProp.PropertyType);

        return MapToForeignKeyColumns(referencedPrimaryKeyColumns, foreignKeyProp);
    }

    private static IList<ColumnProperties> MapToForeignKeyColumns(IList<ColumnProperties> columnProperties, PropertyInfo foreignKeyProp)
    {
        var keyGroup = new ForeignKeyGroup
        {
            AssociatedProperty = foreignKeyProp,
            ReferencedTableName = ExtensionsHelper.GetTableName(foreignKeyProp.DeclaringType!),
            ColumnsNamesPrefix = ExtensionsHelper.GetColumnsNamesPrefix(foreignKeyProp)
        };

        return columnProperties.Select(column => MapToForeignKeyColumn(column, keyGroup)).ToList();
    }

    private static ColumnProperties MapToForeignKeyColumn(ColumnProperties column, ForeignKeyGroup keyGroup)
    {
        var foreignColumn = new ColumnProperties(column)
        {
            Name = $"{keyGroup.ColumnsNamesPrefix}{column.Name}",
            IsForeignKeyColumn = true,
            ForeignKeyGroup = keyGroup,
            IsPrimaryKeyColumn = keyGroup.AssociatedProperty!.IsPrimaryKeyProperty(),
            PropertyName = null
        };

        keyGroup.ReferencedTableName = column.TableName;
        
        keyGroup.KeyPairs.Add(new ForeignKeyPair
        {
            MainColumn = foreignColumn,
            ReferencedColumn = column
        });

        return foreignColumn;
    }
}
using System.Reflection;
using OrmLibrary.Attributes;
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
            Name = entityType.GetCustomAttribute<TableAttribute>()!.Name,
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
            RegisterForeignKeyColumns(property, tableProps);
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
                RegisterForeignKeyColumns(property, tableProps);
            }
        }
        else if (property.IsForeignKeyProperty())
        {
            RegisterForeignKeyColumn(property, tableProps);
        }
        else
        {
            tableProps.RegisterColumn(ExtractColumnProperties(property));
        }
    }

    private static ColumnProperties ExtractColumnProperties(PropertyInfo property)
    {
        var columnBaseType = property.GetBaseType();
        
        return new ColumnProperties
        {
            Name = ExtensionsHelper.GetColumnName(property),
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

    private static void RegisterForeignKeyColumn(PropertyInfo property, TableProperties tableProps)
    {
        var foreignKeyAttribute = property.GetCustomAttribute<ForeignKeyAttribute>()!;
        var referencedProperty = foreignKeyAttribute.ReferencedType.GetProperty(foreignKeyAttribute.ReferencedColumnName)!;
        var mappedColumn = ExtractColumnProperties(property);
        
        mappedColumn.IsForeignKeyColumn = true;
        mappedColumn.ForeignKeyGroup = new ForeignKeyGroup
        {
            AssociatedProperty = referencedProperty,
            KeyPairs = new List<ForeignKeyPair>
            {
                new()
                {
                    MainColumn = mappedColumn,
                    ReferencedColumn = ExtractColumnProperties(referencedProperty)
                }
            },
        };
        
        tableProps.RegisterColumn(mappedColumn);
    }
    
    private static void RegisterForeignKeyColumns(PropertyInfo foreignKeyProp, TableProperties tableProps)
    {
        var referencedPrimaryKeyColumns = GetPrimaryKeysColumns(foreignKeyProp.PropertyType);

        foreach (var column in MapToForeignKeyColumns(referencedPrimaryKeyColumns, foreignKeyProp))
        {
            tableProps.RegisterColumn(column);
        }
    }

    private static IList<ColumnProperties> MapToForeignKeyColumns(IList<ColumnProperties> columnProperties, PropertyInfo foreignKeyProp)
    {
        var keyGroup = new ForeignKeyGroup
        {
            AssociatedProperty = foreignKeyProp
        };

        return columnProperties.Select(column => MapToForeignKeyColumn(column, keyGroup)).ToList();
    }

    private static ColumnProperties MapToForeignKeyColumn(ColumnProperties column, ForeignKeyGroup keyGroup)
    {
        var foreignColumn = new ColumnProperties(column)
        {
            Name = $"{keyGroup.AssociatedProperty.Name}{column.Name}",
            IsForeignKeyColumn = true,
            ForeignKeyGroup = keyGroup,
            IsPrimaryKeyColumn = keyGroup.AssociatedProperty.IsPrimaryKeyProperty(),
            PropertyName = null
        };
        
        keyGroup.KeyPairs.Add(new ForeignKeyPair
        {
            MainColumn = foreignColumn,
            ReferencedColumn = column
        });

        return foreignColumn;
    }
}
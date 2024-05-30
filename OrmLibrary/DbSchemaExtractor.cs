using System.Reflection;
using OrmLibrary.Attributes;
using OrmLibrary.Constraints;
using OrmLibrary.Converters;
using OrmLibrary.Extensions;
using OrmLibrary.SqlServer;

namespace OrmLibrary;

public static class DbSchemaExtractor
{
    private static readonly ISqlTypeConverter _converter = new SqlServerTypeConverter();

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

        Console.WriteLine($"Extracting for {entityType.Name}");
        
        foreach (var property in entityType.GetProperties())
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
        else if (property.IsOneToOneProperty())
        {
                
        }
        else if (property.IsForeignKeyProperty())
        {
            RegisterForeignKeyColumns(property, tableProps);
        }
        else
        {
            tableProps.RegisterColumn(ExtractColumnProperties(property));
        }
    }

    private static void RegisterForeignKeyColumns(PropertyInfo foreignKeyProp, TableProperties tableProps)
    {
        var referencedPrimaryKeyColumns = GetPrimaryKeysColumns(foreignKeyProp.PropertyType);
        var keyGroup = new ForeignKeyGroup
        {
            AssociatedProperty = foreignKeyProp
        };
        
        foreach (var column in referencedPrimaryKeyColumns)
        {
            var referencedColumnName = column.Name;
            
            column.Name = $"{foreignKeyProp.PropertyType.Name}{column.Name}";
            column.IsForeignKeyColumn = true;
            column.ForeignKeyGroup = keyGroup;
            
            keyGroup.KeyPairs.Add(new ForeignKeyPair
            {
                ColumnName = column.Name,
                ReferencedColumnName = referencedColumnName
            });
            
            tableProps.RegisterColumn(column);
        }
    }

    private static ColumnProperties ExtractColumnProperties(PropertyInfo columnProperties)
    {
        var columnBaseType = columnProperties.GetBaseType();
        
        return new ColumnProperties
        {
            Name = ExtensionsHelper.GetColumnName(columnProperties),
            PropertyName = columnProperties.Name,
            IsPrimaryKeyColumn = columnProperties.GetCustomAttribute<PrimaryKeyAttribute>() != null,
            IsNullable = columnProperties.IsNullable(),
            LanguageNativeType = columnBaseType,
            SqlColumnType = _converter.ConvertToSqlType(columnBaseType)
        };
    }

    private static readonly IDictionary<Type, List<ColumnProperties>> primaryKeysCache = new Dictionary<Type, List<ColumnProperties>>();
    
    private static IList<ColumnProperties> GetPrimaryKeysColumns(Type entityType)
    {
        if (primaryKeysCache.TryGetValue(entityType, out var primaryKeys))
        {
            return primaryKeys;
        }

        primaryKeys = new List<ColumnProperties>();
        foreach (var property in ExtensionsHelper.GetPrimaryKeyProperties(entityType))
        {
            if (property.PropertyType.IsMappedEntityType())
            {
                primaryKeys.AddRange(GetPrimaryKeysColumns(property.PropertyType));
            }
            else
            {
                primaryKeys.Add(ExtractColumnProperties(property));
            }
        }

        primaryKeysCache.Add(entityType, primaryKeys);
        
        return primaryKeys;
    }
}
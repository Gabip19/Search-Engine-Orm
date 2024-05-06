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

        foreach (var property in entityType.GetProperties())
        {
            if (ExtensionsHelper.IsForeignKeyReference(property))
            {
                RegisterForeignKeyColumns(property, tableProps);
            }
            else
            {
                tableProps.RegisterColumn(ExtractColumnProperties(property));
            }
        }

        return tableProps;
    }

    private static void RegisterForeignKeyColumns(PropertyInfo foreignKeyProp, TableProperties tableProps)
    {
        var referencedPrimaryKeys = ExtensionsHelper.GetPrimaryKeyProperties(foreignKeyProp.PropertyType);
        var keyGroup = new ForeignKeyGroup
        {
            AssociatedProperty = foreignKeyProp
        };
        
        if (referencedPrimaryKeys.Count > 0)
        {
            foreach (var primaryKey in referencedPrimaryKeys)
            {
                var column = ExtractColumnProperties(primaryKey);
                
                column.Name = $"{foreignKeyProp.PropertyType}{column.Name}";
                column.IsForeignKeyColumn = true;
                column.ForeignKeyGroup = keyGroup;
                
                keyGroup.KeyPairs.Add(new ForeignKeyPair
                {
                    ColumnName = column.Name,
                    ReferencedColumnName = ExtensionsHelper.GetColumnName(primaryKey)
                });
                
                tableProps.RegisterColumn(column);
            }
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
}
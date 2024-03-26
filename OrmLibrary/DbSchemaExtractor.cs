using System.Reflection;
using OrmLibrary.Attributes;
using OrmLibrary.Converters;
using OrmLibrary.Extensions;
using OrmLibrary.SqlServer;

namespace OrmLibrary;

public static class DbSchemaExtractor
{
    private static readonly ISqlTypeConverter _converter = new SqlServerTypeConverter();

    public static IEnumerable<TableProperties> ExtractTablesProperties(IEnumerable<Type> entitiesTypes)
    {
        return entitiesTypes.Select(ExtractTableProperties).ToList();
    }
    
    private static TableProperties ExtractTableProperties(Type entityType)
    {
        var tableProps = new TableProperties
        {
            Name = entityType.GetCustomAttribute<TableAttribute>()!.Name,
        };

        foreach (var property in entityType.GetProperties())
        {
            tableProps.RegisterColumn(ExtractColumnProperties(property));
        }

        return tableProps;
    }
    
    private static ColumnProperties ExtractColumnProperties(PropertyInfo columnProperties)
    {
        var columnBaseType = columnProperties.GetBaseType();
        
        return new ColumnProperties
        {
            Name = columnProperties.GetCustomAttribute<ColumnAttribute>()?.Name ?? columnProperties.Name,
            IsPrimaryKeyColumn = columnProperties.GetCustomAttribute<PrimaryKeyAttribute>() != null,
            IsNullable = columnProperties.IsNullable(),
            LanguageNativeType = columnBaseType,
            SqlColumnType = _converter.ConvertToSqlType(columnBaseType)
        };
    }
}
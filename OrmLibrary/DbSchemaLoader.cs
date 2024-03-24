using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using OrmLibrary.Converters;
using OrmLibrary.Extensions;
using OrmLibrary.SqlServer;

namespace OrmLibrary;

public class DbSchemaExtractor
{
    // public IEnumerable<TableProperties> ExtractTablesProperties(IEnumerable<Type> entitiesTypes)
    // {
    //     entitiesTypes.Select(type => ExtractTableProperties(type));
    // }
    //
    // private TableProperties ExtractTableProperties(Type entityType)
    // {        
    //     
    // }

    private static readonly ISqlTypeConverter _converter = new SqlServerTypeConverter();
    
    public static ColumnProperties ExtractColumnProperties(PropertyInfo columnProperties)
    {
        var columnBaseType = columnProperties.GetBaseType();
        
        return new ColumnProperties
        {
            Name = columnProperties.GetCustomAttribute<ColumnAttribute>()?.Name ?? columnProperties.Name,
            IsNullable = columnProperties.IsNullable(),
            LanguageNativeType = columnBaseType,
            SqlColumnType = _converter.ConvertToSqlType(columnBaseType)
        };
    }
}
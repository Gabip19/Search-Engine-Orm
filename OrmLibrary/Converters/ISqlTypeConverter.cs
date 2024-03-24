using OrmLibrary.Enums;

namespace OrmLibrary.Converters;

public interface ISqlTypeConverter
{
    IReadOnlyDictionary<Type, SqlType> TypeMappings { get; }
    
    SqlType ConvertToSqlType(Type nativeType);
}
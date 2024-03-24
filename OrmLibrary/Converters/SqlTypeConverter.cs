using OrmLibrary.Enums;

namespace OrmLibrary.Converters;

public abstract class SqlTypeConverter : ISqlTypeConverter
{
    public abstract IReadOnlyDictionary<Type, SqlType> TypeMappings { get; }

    public virtual SqlType ConvertToSqlType(Type nativeType)
    {
        if (TypeMappings.TryGetValue(nativeType, out var sqlType))
        {
            return sqlType;
        }

        var underlyingType = Nullable.GetUnderlyingType(nativeType);
        if (underlyingType is not null)
        {
            return ConvertToSqlType(underlyingType);
        }

        throw new ArgumentException($"No converting type defined for {nativeType.FullName}");
    }
}
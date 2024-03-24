using System.Collections.ObjectModel;
using OrmLibrary.Converters;
using OrmLibrary.Enums;

namespace OrmLibrary.SqlServer;

public class SqlServerTypeConverter : SqlTypeConverter
{
    public override IReadOnlyDictionary<Type, SqlType> TypeMappings { get; } =
        new ReadOnlyDictionary<Type, SqlType>(new Dictionary<Type, SqlType>
        {
            { typeof(bool), SqlType.Bit },
            { typeof(byte), SqlType.TinyInt },
            { typeof(short), SqlType.SmallInt },
            { typeof(int), SqlType.Int },
            { typeof(long), SqlType.BigInt },
            { typeof(float), SqlType.Real },
            { typeof(double), SqlType.Float },
            { typeof(decimal), SqlType.Decimal },
            { typeof(char), SqlType.NChar },
            { typeof(string), SqlType.NVarChar },
            { typeof(DateTime), SqlType.DateTime2 },
            { typeof(Guid), SqlType.UniqueIdentifier },
            { typeof(byte[]), SqlType.VarBinary }
        });
}
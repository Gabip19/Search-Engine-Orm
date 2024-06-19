using Newtonsoft.Json;
using OrmLibrary.Constraints;
using OrmLibrary.Extensions;

namespace OrmLibrary.Serialization.Converters;

public class CustomConstraintConverter : JsonConverter<ITableConstraint>
{
    public override void WriteJson(JsonWriter writer, ITableConstraint? value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("type");
        writer.WriteValue(value.GetType().Name);

        writer.WritePropertyName("name");
        writer.WriteValue(value.Name);

        switch (value)
        {
            case ForeignKeyConstraint fkConstraint:
                writer.WritePropertyName("referencedTableName");
                writer.WriteValue(fkConstraint.ReferencedTableName);

                writer.WritePropertyName("foreignKeyGroup");
                serializer.Serialize(writer, fkConstraint.ForeignKeyGroup.MapToDto(), typeof(ForeignKeyGroupDto));
                break;
            
            case UniqueConstraint uqConstraint:
                writer.WritePropertyName("columnName");
                writer.WriteValue(uqConstraint.ColumnName);
                break;
        }

        writer.WriteEndObject();
    }

    public override ITableConstraint ReadJson(JsonReader reader, Type objectType, ITableConstraint? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
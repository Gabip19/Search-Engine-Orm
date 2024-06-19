using Newtonsoft.Json;
using OrmLibrary.Mappings;

namespace OrmLibrary.Serialization.Converters;

public class CustomTablePropertiesConverter : JsonConverter<TableProperties>
{
    public override void WriteJson(JsonWriter writer, TableProperties value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("name");
        writer.WriteValue(value.Name);

        writer.WritePropertyName("associatedType");
        writer.WriteValue(value.AssociatedType.ToString());

        writer.WritePropertyName("columns");
        writer.WriteStartArray();
        var columnSerializer = new CustomColumnPropertiesConverter();
        foreach (var column in value.Columns)
        {
            columnSerializer.WriteJson(writer, column, serializer);
        }
        writer.WriteEndArray();

        writer.WritePropertyName("constraints");
        writer.WriteStartArray();
        var constraintSerializer = new CustomConstraintConverter();
        foreach (var constraint in value.Constraints)
        {
            constraintSerializer.WriteJson(writer, constraint, serializer);
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    public override TableProperties ReadJson(JsonReader reader, Type objectType, TableProperties existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
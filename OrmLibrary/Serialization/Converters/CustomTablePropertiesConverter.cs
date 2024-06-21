using Newtonsoft.Json;
using OrmLibrary.Extensions;
using OrmLibrary.Mappings;

namespace OrmLibrary.Serialization.Converters;

public class CustomTablePropertiesConverter : JsonConverter<TableProperties>
{
    public override void WriteJson(JsonWriter writer, TableProperties? value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("name");
        writer.WriteValue(value.Name);

        writer.WritePropertyName("associatedType");
        writer.WriteValue($"{value.AssociatedType},{value.AssociatedType!.Assembly.GetName().Name}");

        writer.WritePropertyName("columns");
        writer.WriteStartArray();
        var columnSerializer = new CustomColumnPropertiesConverter();
        foreach (var column in value.Columns)
        {
            columnSerializer.WriteJson(writer, column, serializer);
        }
        writer.WriteEndArray();

        writer.WritePropertyName("tableReferences");
        writer.WriteStartArray();
        foreach (var fkGroup in value.ForeignKeys)
        {
            serializer.Serialize(writer, fkGroup.MapToDto(), typeof(ForeignKeyGroupDto));
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    public override TableProperties ReadJson(JsonReader reader, Type objectType, TableProperties? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
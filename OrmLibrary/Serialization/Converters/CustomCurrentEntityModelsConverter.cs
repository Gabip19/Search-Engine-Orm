using Newtonsoft.Json;
using OrmLibrary.Extensions;
using OrmLibrary.Mappings;

namespace OrmLibrary.Serialization.Converters;

public class CustomCurrentEntityModelsConverter : JsonConverter<CurrentEntityModels>
{
    public override void WriteJson(JsonWriter writer, CurrentEntityModels? value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("lastDbUpdate");
        writer.WriteValue(value.LastDbUpdate);

        writer.WritePropertyName("currentDbVersion");
        writer.WriteValue(value.CurrentDbVersion);

        writer.WritePropertyName("tablesMappings");
        writer.WriteStartArray();
        var tablePropertiesConverter = new CustomTablePropertiesConverter();
        foreach (var mapping in value.EntitiesMappings)
        {
            tablePropertiesConverter.WriteJson(writer, mapping.Value, serializer);
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    public override CurrentEntityModels ReadJson(JsonReader reader, Type objectType, CurrentEntityModels? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var models = new CurrentEntityModels();
        var tablePropertiesConverter = new CustomTablePropertiesDtoConverter();
        var dtoMappings = new Dictionary<string, TablePropertiesDto>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return models;
            }

            var propertyName = reader.Value.ToString();
            reader.Read();

            switch (propertyName)
            {
                case "lastDbUpdate":
                    models.LastDbUpdate = DateTime.Parse(reader.Value.ToString());
                    break;
                case "currentDbVersion":
                    models.CurrentDbVersion = int.Parse(reader.Value.ToString());
                    break;
                case "tablesMappings":
                    reader.Read();
                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        var tablePropertiesDto = tablePropertiesConverter.ReadJson(reader, typeof(TablePropertiesDto), null, false, serializer);
                        dtoMappings.Add(tablePropertiesDto.UnlinkedTableProperties.Name, tablePropertiesDto);
                        reader.Read();
                    }

                    models.EntitiesMappings = MappingExtensions.MapToTableProperties(dtoMappings)
                        .ToDictionary(properties => properties.AssociatedType);
                    
                    break;
            }
        }

        throw new JsonSerializationException("Unexpected end when reading CurrentEntityModels.");
    }
}
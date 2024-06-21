using Newtonsoft.Json;
using OrmLibrary.Mappings;

namespace OrmLibrary.Serialization.Converters;

public class CustomTablePropertiesDtoConverter : JsonConverter<TablePropertiesDto>
{
    public override void WriteJson(JsonWriter writer, TablePropertiesDto? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override TablePropertiesDto ReadJson(JsonReader reader, Type objectType, TablePropertiesDto? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var tableProps = new TableProperties();
        var foreignKeyGroupsDtos = new List<ForeignKeyGroupDto>();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return new TablePropertiesDto
                {
                    UnlinkedTableProperties = tableProps,
                    KeyGroupsDtos = foreignKeyGroupsDtos
                };
            }

            var propertyName = reader.Value.ToString();
            reader.Read();

            switch (propertyName)
            {
                case "name":
                    tableProps.Name = reader.Value.ToString();
                    break;
                case "associatedType":
                    tableProps.AssociatedType = Type.GetType(reader.Value.ToString());
                    break;
                case "columns":
                    ReadColumns(tableProps, reader, serializer);
                    break;
                case "tableReferences":
                    ReadForeignKeyGroupDtos(foreignKeyGroupsDtos, tableProps, reader, serializer);
                    break;
            }
        }

        throw new JsonSerializationException("Unexpected end when reading TableProperties.");
    }
    
    private static void ReadColumns(TableProperties table, JsonReader reader, JsonSerializer serializer)
    {
        reader.Read();
        var columnSerializer = new CustomColumnPropertiesConverter();
        while (reader.TokenType != JsonToken.EndArray)
        {
            var column = columnSerializer.ReadJson(reader, typeof(ColumnProperties), null, false, serializer);
            table.RegisterColumn(column);
            reader.Read();
        }
    }

    private static void ReadForeignKeyGroupDtos(ICollection<ForeignKeyGroupDto> foreignKeyGroupsDtos, TableProperties tableProps, JsonReader reader, JsonSerializer serializer)
    {
        reader.Read();
        while (reader.TokenType != JsonToken.EndArray)
        {
            var foreignKeyGroupDto = serializer.Deserialize<ForeignKeyGroupDto>(reader)!;
            foreignKeyGroupDto.AssociatedTableName = tableProps.Name;
            foreignKeyGroupsDtos.Add(foreignKeyGroupDto);
            reader.Read();
        }
    }
}
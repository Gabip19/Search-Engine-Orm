using Newtonsoft.Json;
using OrmLibrary.Enums;
using OrmLibrary.Mappings;

namespace OrmLibrary.Serialization.Converters;

public class CustomColumnPropertiesConverter : JsonConverter<ColumnProperties>
{
    public override void WriteJson(JsonWriter writer, ColumnProperties? value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("name");
        writer.WriteValue(value.Name);

        writer.WritePropertyName("propertyName");
        writer.WriteValue(value.PropertyName);

        writer.WritePropertyName("languageNativeType");
        writer.WriteValue(value.LanguageNativeType.ToString());

        writer.WritePropertyName("sqlColumnType");
        writer.WriteValue(value.SqlColumnType.ToString());
        
        writer.WritePropertyName("isNullable");
        writer.WriteValue(value.IsNullable);

        if (value.IsPrimaryKeyColumn)
        {
            writer.WritePropertyName("isPrimaryKeyColumn");
            writer.WriteValue(value.IsPrimaryKeyColumn);
        }

        if (value.IsForeignKeyColumn)
        {
            writer.WritePropertyName("isForeignKeyColumn");
            writer.WriteValue(value.IsForeignKeyColumn);
        }

        if (value.IsUnique)
        {
            writer.WritePropertyName("isUnique");
            writer.WriteValue(value.IsUnique);
        }

        if (value.IsFixedLength.HasValue)
        {
            writer.WritePropertyName("isFixedLength");
            writer.WriteValue(value.IsFixedLength.Value);
        }

        if (value.MaxLength.HasValue)
        {
            writer.WritePropertyName("maxLength");
            writer.WriteValue(value.MaxLength.Value);
        }

        if (value.Precision.HasValue)
        {
            writer.WritePropertyName("precision");
            writer.WriteValue(value.Precision.Value);
        }

        if (value.DefaultValue != null)
        {
            writer.WritePropertyName("defaultValue");
            writer.WriteValue(value.DefaultValue.ToString());
        }

        if (value.ComputedColumnSql != null)
        {
            writer.WritePropertyName("computedColumnSql");
            writer.WriteValue(value.ComputedColumnSql);
        }

        writer.WriteEndObject();
    }

    public override ColumnProperties ReadJson(JsonReader reader, Type objectType, ColumnProperties? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var column = new ColumnProperties();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return column;
            }

            var propertyName = reader.Value?.ToString() ?? 
                               throw new ArgumentException("Invalid JSON property name.");
            
            reader.Read();

            switch (propertyName)
            {
                case "name":
                    column.Name = reader.Value.ToString();
                    break;
                case "propertyName":
                    column.PropertyName = reader.Value?.ToString();
                    break;
                case "languageNativeType":
                    column.LanguageNativeType = Type.GetType(reader.Value.ToString());
                    break;
                case "sqlColumnType":
                    column.SqlColumnType = (SqlType)Enum.Parse(typeof(SqlType), reader.Value.ToString());
                    break;
                case "isNullable":
                    column.IsNullable = bool.Parse(reader.Value.ToString());
                    break;
                case "isPrimaryKeyColumn":
                    column.IsPrimaryKeyColumn = bool.Parse(reader.Value.ToString());
                    break;
                case "isForeignKeyColumn":
                    column.IsForeignKeyColumn = bool.Parse(reader.Value.ToString());
                    break;
                case "isUnique":
                    column.IsUnique = bool.Parse(reader.Value.ToString());
                    break;
                case "isFixedLength":
                    column.IsFixedLength = bool.Parse(reader.Value.ToString());
                    break;
                case "maxLength":
                    column.MaxLength = int.Parse(reader.Value.ToString());
                    break;
                case "precision":
                    column.Precision = int.Parse(reader.Value.ToString());
                    break;
                case "defaultValue":
                    column.DefaultValue = reader.Value;
                    break;
                case "computedColumnSql":
                    column.ComputedColumnSql = reader.Value.ToString();
                    break;
            }
        }

        throw new JsonSerializationException("Unexpected end when reading ColumnProperties.");
    }
}
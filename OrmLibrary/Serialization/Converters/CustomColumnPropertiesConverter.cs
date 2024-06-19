using Newtonsoft.Json;
using OrmLibrary.Mappings;

namespace OrmLibrary.Serialization.Converters;

public class CustomColumnPropertiesConverter : JsonConverter<ColumnProperties>
{
    public override void WriteJson(JsonWriter writer, ColumnProperties value, JsonSerializer serializer)
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

    public override ColumnProperties ReadJson(JsonReader reader, Type objectType, ColumnProperties existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
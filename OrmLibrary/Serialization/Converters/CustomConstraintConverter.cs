using Newtonsoft.Json;
using OrmLibrary.Constraints;

namespace OrmLibrary.Serialization.Converters;

public class CustomConstraintConverter : JsonConverter<ITableConstraint>
{
    public override void WriteJson(JsonWriter writer, ITableConstraint value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("type");
        writer.WriteValue(value.GetType().Name);

        writer.WritePropertyName("name");
        writer.WriteValue(value.Name);

        if (value is ForeignKeyConstraint fkConstraint)
        {
            writer.WritePropertyName("referencedTableName");
            writer.WriteValue(fkConstraint.ReferencedTableName);

            writer.WritePropertyName("foreignKeyGroup");
            writer.WriteStartObject();

            writer.WritePropertyName("associatedProperty");
            writer.WriteValue(fkConstraint.ForeignKeyGroup.AssociatedProperty.Name);

            writer.WritePropertyName("columns");
            writer.WriteStartArray();
            foreach (var keyPair in fkConstraint.ForeignKeyGroup.KeyPairs)
            {
                writer.WriteValue(keyPair.MainColumn.Name);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("referencedColumns");
            writer.WriteStartArray();
            foreach (var keyPair in fkConstraint.ForeignKeyGroup.KeyPairs)
            {
                writer.WriteValue(keyPair.ReferencedColumn.Name);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        if (value is UniqueConstraint uqConstraint)
        {
            writer.WritePropertyName("columnName");
            writer.WriteValue(uqConstraint.ColumnName);
        }

        writer.WriteEndObject();
    }

    public override ITableConstraint ReadJson(JsonReader reader, Type objectType, ITableConstraint existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
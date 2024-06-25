using Newtonsoft.Json;
using OrmLibrary.Migrations;
using OrmLibrary.Migrations.MigrationOperations;

namespace OrmLibrary.Serialization.Converters;

public class DbMigrationConverter : JsonConverter<DbMigration>
{
    public override void WriteJson(JsonWriter writer, DbMigration? value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("MigrationId");
        writer.WriteValue(value!.MigrationId);

        writer.WritePropertyName("DbVersion");
        writer.WriteValue(value.DbVersion);

        writer.WritePropertyName("MigrationDate");
        writer.WriteValue(value.MigrationDate.ToString("o"));

        writer.WritePropertyName("Operations");
        serializer.Serialize(writer, value.Operations, typeof(MigrationOperationsCollection));
        
        writer.WriteEndObject();
    }

    public override DbMigration ReadJson(JsonReader reader, Type objectType, DbMigration? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var dbMigration = new DbMigration();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(dbMigration.MigrationId):
                    dbMigration.MigrationId = reader.Value.ToString()!;
                    break;
                case nameof(dbMigration.DbVersion):
                    dbMigration.DbVersion = int.Parse(reader.Value.ToString()!);
                    break;
                case nameof(dbMigration.MigrationDate):
                    dbMigration.MigrationDate = DateTime.Parse(reader.Value.ToString()!);
                    break;
                case nameof(dbMigration.Operations):
                    dbMigration.Operations = serializer.Deserialize<MigrationOperationsCollection>(reader)!;
                    break;
            }
            
        }

        return dbMigration;
    }
}
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

    public override DbMigration ReadJson(JsonReader reader, Type objectType, DbMigration existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
        // JObject obj = JObject.Load(reader);
        //
        // var dbMigration = new DbMigration
        // {
        //     MigrationId = (string)obj["MigrationId"],
        //     DbVersion = (int)obj["DbVersion"],
        //     MigrationDate = (DateTime)obj["MigrationDate"]
        // };
        //
        // var operationsArray = (JArray)obj["Operations"];
        // var operationsCollection = new MigrationOperationsCollection();
        //
        // foreach (var operationToken in operationsArray)
        // {
        //     var operationObj = (JObject)operationToken;
        //     var type = (string)operationObj["Type"];
        //     var tableName = (string)operationObj["TableName"];
        //
        //     switch (type)
        //     {
        //         case "Create":
        //             var addOperation = new AddTableMigrationOperation
        //             {
        //                 TableName = tableName,
        //                 Columns = operationObj["Columns"].ToObject<IList<ColumnDefinition>>()
        //             };
        //             operationsCollection.Add(addOperation);
        //             break;
        //         case "Drop":
        //             var dropOperation = new DropTableMigrationOperation
        //             {
        //                 TableName = tableName
        //             };
        //             operationsCollection.Add(dropOperation);
        //             break;
        //         case "Update":
        //             var alterOperation = new AlterTableMigrationOperation
        //             {
        //                 TableName = tableName,
        //                 Operations = operationObj["Operations"].ToObject<IList<ITableMigrationOperation>>(serializer)
        //             };
        //             operationsCollection.Add(alterOperation);
        //             break;
        //     }
        // }
        //
        // dbMigration.Operations = operationsCollection;
        //
        // return dbMigration;
    }
}
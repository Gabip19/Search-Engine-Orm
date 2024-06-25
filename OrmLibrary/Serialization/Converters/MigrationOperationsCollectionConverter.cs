using Newtonsoft.Json;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations;
using OrmLibrary.Migrations.MigrationOperations.Tables;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

namespace OrmLibrary.Serialization.Converters;

public class MigrationOperationsCollectionConverter : JsonConverter<MigrationOperationsCollection>
{
    public override void WriteJson(JsonWriter writer, MigrationOperationsCollection? value, JsonSerializer serializer)
    {
        writer.WriteStartArray();

        WriteOperations(writer, value.AddTableOperations, serializer);
        WriteOperations(writer, value.DropTableOperations, serializer);
        WriteOperations(writer, value.AlterTableOperations, serializer);

        writer.WriteEndArray();
    }

    private void WriteOperations(JsonWriter writer, IEnumerable<IAddTableMigrationOperation> operations, JsonSerializer serializer)
    {
        foreach (var operation in operations)
        {
            writer.WriteStartObject();
            
            writer.WritePropertyName("Type");
            writer.WriteValue(operation.OperationType.ToString());
            
            writer.WritePropertyName("TableName");
            writer.WriteValue(operation.TableName);

            writer.WritePropertyName("Columns");
            writer.WriteStartArray();

            foreach (var column in operation.Columns)
            {
                serializer.Serialize(writer, column, typeof(ColumnProperties));
            }
            
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }

    private void WriteOperations(JsonWriter writer, IEnumerable<IDropTableMigrationOperation> operations, JsonSerializer serializer)
    {
        foreach (var operation in operations)
        {
            writer.WriteStartObject();
            
            writer.WritePropertyName("Type");
            writer.WriteValue(operation.OperationType.ToString());
            
            writer.WritePropertyName("TableName");
            writer.WriteValue(operation.TableName);

            writer.WriteEndObject();
        }
    }
    
    private void WriteOperations(JsonWriter writer, IEnumerable<IAlterTableMigrationOperation> operations, JsonSerializer serializer)
    {
        foreach (var operation in operations)
        {
            switch (operation.OperationType)
            {
                case TableOperationType.AlterTable:
                    WriteAlterTableStructureOperation(writer, operation, serializer);
                    break;
                case TableOperationType.AlterForeignKey:
                case TableOperationType.AlterPrimaryKey:
                case TableOperationType.AddConstraint:
                case TableOperationType.DropConstraint:
                    serializer.Serialize(writer, (operation as IConstraintMigrationOperation)!, typeof(IConstraintMigrationOperation));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation.OperationType), operation.OperationType,
                        "Can not convert the provided operation type.");
            }
        }
    }

    private static void WriteAlterTableStructureOperation(JsonWriter writer, IAlterTableMigrationOperation operation, JsonSerializer serializer)
    {
        writer.WriteStartObject();
            
        writer.WritePropertyName("Type");
        writer.WriteValue(operation.OperationType.ToString());
        
        writer.WritePropertyName("TableName");
        writer.WriteValue(operation.TableName);
        
        writer.WritePropertyName("Operations");
        serializer.Serialize(writer, (operation as AlterTableStructureOperation)!.ColumnOperations, typeof(ColumnsOperationsCollection));
                    
        writer.WriteEndObject();
    }

    public override MigrationOperationsCollection ReadJson(JsonReader reader, Type objectType, MigrationOperationsCollection? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
        // var operationsCollection = new MigrationOperationsCollection();
        // var operationsArray = JArray.Load(reader);
        //
        // foreach (var operationToken in operationsArray)
        // {
        //     var operationObj = (JObject)operationToken;
        //     var type = (string)operationObj["Type"];
        //     var tableName = (string)operationObj["TableName"];
        //
        //     switch (type)
        //     {
        //         case "AddTable":
        //             operationsCollection.Add(DeserializeAddTableOperation(operationObj, tableName));
        //             break;
        //         case "DropTable":
        //             operationsCollection.Add(DeserializeDropTableOperation(operationObj, tableName));
        //             break;
        //         case "AlterForeignKey":
        //         case "AlterPrimaryKey":
        //         case "AlterTable":
        //         case "AddConstraint":
        //         case "DropConstraint":
        //             operationsCollection.Add(DeserializeAlterTableOperation(operationObj, type, tableName, serializer));
        //             break;
        //         default:
        //             throw new NotSupportedException($"Unsupported operation type: {type}");
        //     }
        // }
        //
        // return operationsCollection;
    }

    // private IAddTableMigrationOperation DeserializeAddTableOperation(JObject operationObj, string tableName)
    // {
    //     return new AddTableMigrationOperation
    //     {
    //         TableName = tableName,
    //         Columns = operationObj["Columns"].ToObject<IList<ColumnDefinition>>()
    //     };
    // }

    // private IDropTableMigrationOperation DeserializeDropTableOperation(JObject operationObj, string tableName)
    // {
    //     return new DropTableMigrationOperation
    //     {
    //         TableName = tableName
    //     };
    // }

    // private IAlterTableMigrationOperation DeserializeAlterTableOperation(JObject operationObj, string type, string tableName, JsonSerializer serializer)
    // {
    //     var alterOperation = new AlterTableMigrationOperation
    //     {
    //         TableName = tableName,
    //         Operations = new List<ITableMigrationOperation>()
    //     };
    //
    //     var subOperationsArray = (JArray)operationObj["Operations"];
    //
    //     foreach (var subOperationToken in subOperationsArray)
    //     {
    //         var subOperationObj = (JObject)subOperationToken;
    //         var subType = (string)subOperationObj["Type"];
    //
    //         switch (subType)
    //         {
    //             case "AddColumn":
    //                 alterOperation.Operations.Add(new AddColumnOperation
    //                 {
    //                     TableName = tableName,
    //                     Column = subOperationObj["Column"].ToObject<ColumnDefinition>()
    //                 });
    //                 break;
    //             case "DropColumn":
    //                 alterOperation.Operations.Add(new DropColumnOperation
    //                 {
    //                     TableName = tableName,
    //                     ColumnName = (string)subOperationObj["ColumnName"]
    //                 });
    //                 break;
    //             case "ChangeMaxLength":
    //                 alterOperation.Operations.Add(new ChangeMaxLengthColumnOperation
    //                 {
    //                     TableName = tableName,
    //                     ColumnName = (string)subOperationObj["ColumnName"],
    //                     MaxLength = (int)subOperationObj["MaxLength"]
    //                 });
    //                 break;
    //             case "ChangeNullability":
    //                 alterOperation.Operations.Add(new ChangeNullabilityColumnOperation
    //                 {
    //                     TableName = tableName,
    //                     ColumnName = (string)subOperationObj["ColumnName"],
    //                     IsNullable = (bool)subOperationObj["IsNullable"]
    //                 });
    //                 break;
    //             case "ChangePrimaryKey":
    //                 alterOperation.Operations.Add(new ChangePrimaryKeyColumnOperation
    //                 {
    //                     TableName = tableName,
    //                     ColumnName = (string)subOperationObj["ColumnName"],
    //                     IsPrimaryKey = (bool)subOperationObj["IsPrimaryKey"]
    //                 });
    //                 break;
    //             case "ChangeDataType":
    //                 alterOperation.Operations.Add(new ChangeTypeColumnOperation
    //                 {
    //                     TableName = tableName,
    //                     ColumnName = (string)subOperationObj["ColumnName"],
    //                     DataType = (string)subOperationObj["DataType"]
    //                 });
    //                 break;
    //             case "RenameColumn":
    //                 alterOperation.Operations.Add(new RenameColumnOperation
    //                 {
    //                     TableName = tableName,
    //                     OldName = (string)subOperationObj["OldName"],
    //                     NewName = (string)subOperationObj["NewName"]
    //                 });
    //                 break;
    //             default:
    //                 throw new NotSupportedException($"Unsupported sub-operation type: {subType}");
    //         }
    //     }
    //
    //     return alterOperation;
    // }

    // public override bool CanConvert(Type objectType)
    // {
    //     return objectType == typeof(MigrationOperationsCollection);
    // }
}
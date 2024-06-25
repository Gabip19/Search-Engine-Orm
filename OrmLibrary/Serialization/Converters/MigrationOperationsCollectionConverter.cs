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
        var collection = new MigrationOperationsCollection();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndArray)
            {
                break;
            }
        
            if (reader.TokenType == JsonToken.StartObject)
            {
                reader.Read();
                
                var typePropertyName = reader.Value?.ToString() ??
                                       throw new JsonSerializationException("Operation type property missing.");
                if (typePropertyName != "Type")
                {
                    throw new JsonSerializationException("Operation type property missing.");
                }
        
                reader.Read();
                var operationType = Enum.Parse<TableOperationType>(reader.Value.ToString()!);
        
                switch (operationType)
                {
                    case TableOperationType.AddTable:
                        collection.Add(ReadAddTableOperation(reader, serializer));
                        break;
                    case TableOperationType.DropTable:
                        collection.Add(ReadDropTableOperation(reader));
                        break;
                    case TableOperationType.AlterTable:
                        collection.Add(ReadAlterTableStructureOperation(reader, serializer));
                        break;
                    case TableOperationType.AlterForeignKey:
                    case TableOperationType.AlterPrimaryKey:
                    case TableOperationType.AddConstraint:
                    case TableOperationType.DropConstraint:
                        collection.Add(ReadConstraintOperation(reader, operationType, serializer));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(operationType),
                            $"Invalid column operation type provided: {operationType}");
                }
            }
        }
        
        return collection;
    }
    
    private static AlterTableStructureOperation ReadAlterTableStructureOperation(JsonReader reader, JsonSerializer serializer)
    {
        var operation = new AlterTableStructureOperation
        {
            OperationType = TableOperationType.AlterTable
        };
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(AlterTableStructureOperation.TableName):
                    operation.TableName = reader.Value.ToString()!;
                    break;
                case "Operations":
                    operation.ColumnOperations = serializer.Deserialize<ColumnsOperationsCollection>(reader)!;
                    break;
            }
        }
        
        foreach (var columnOperation in operation.ColumnOperations.AddColumnOperations)
        {
            columnOperation.TableName = operation.TableName;
        }
        
        foreach (var columnOperation in operation.ColumnOperations.DropColumnOperations)
        {
            columnOperation.TableName = operation.TableName;
        }
        
        foreach (var columnOperation in operation.ColumnOperations.AlterColumnOperations)
        {
            columnOperation.TableName = operation.TableName;
        }

        return operation;
    }
    
    private static IDropTableMigrationOperation ReadDropTableOperation(JsonReader reader)
    {
        var operation = new DropTableOperation
        {
            OperationType = TableOperationType.DropTable
        };
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            operation.TableName = propertyName switch
            {
                nameof(AlterTableStructureOperation.TableName) => reader.Value.ToString()!,
                _ => operation.TableName
            };
        }

        return operation;
    }
    
    private static IAddTableMigrationOperation ReadAddTableOperation(JsonReader reader, JsonSerializer serializer)
    {
        var operation = new AddTableOperation
        {
            OperationType = TableOperationType.AddTable,
            Columns = new List<ColumnProperties>()
        };
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(AlterTableStructureOperation.TableName):
                    operation.TableName = reader.Value.ToString()!;
                    break;
                case "Columns":
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.EndArray) break;

                        var columnProps = serializer.Deserialize<ColumnProperties>(reader)!;
                        operation.Columns.Add(columnProps);
                    }
                    break;
            }
        }
        
        return operation;
    }

    private static IConstraintMigrationOperation ReadConstraintOperation(JsonReader reader, TableOperationType operationType, JsonSerializer serializer)
    {
        var constraintConverter = new ConstraintMigrationOperationConverter();
        return constraintConverter.ReadJson(
            reader,
            typeof(IConstraintMigrationOperation),
            new DropConstraintOperation { OperationType = operationType },
            true,
            serializer
        );
    }
}
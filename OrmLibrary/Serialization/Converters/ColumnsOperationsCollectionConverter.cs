using Newtonsoft.Json;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations;
using OrmLibrary.Migrations.MigrationOperations.Columns;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

namespace OrmLibrary.Serialization.Converters;

public class ColumnsOperationsCollectionConverter : JsonConverter<ColumnsOperationsCollection>
{
    public override void WriteJson(JsonWriter writer, ColumnsOperationsCollection? value, JsonSerializer serializer)
    {
        writer.WriteStartArray();

        WriteOperations(writer, value.AddColumnOperations, serializer);
        WriteOperations(writer, value.DropColumnOperations, serializer);
        WriteOperations(writer, value.AlterColumnOperations, serializer);

        writer.WriteEndArray();
    }

    private void WriteOperations(JsonWriter writer, IList<IAddColumnMigrationOperation> operations, JsonSerializer serializer)
    {
        if (!operations.Any())
        {
            return;
        }
        
        writer.WriteStartObject();
        
        writer.WritePropertyName("Type");
        writer.WriteValue(ColumnOperationType.AddColumn.ToString());

        writer.WritePropertyName("Columns");
        writer.WriteStartArray();
        
        foreach (var operation in operations)
        {
            serializer.Serialize(writer, operation.NewColumnProps, typeof(ColumnProperties));
        }
        
        writer.WriteEndArray();
        
        writer.WriteEndObject();
    }

    private void WriteOperations(JsonWriter writer, IList<IDropColumnMigrationOperation> operations, JsonSerializer serializer)
    {
        if (!operations.Any())
        {
            return;
        }
        
        writer.WriteStartObject();
        
        writer.WritePropertyName("Type");
        writer.WriteValue(ColumnOperationType.DropColumn.ToString());

        writer.WritePropertyName("Columns");
        writer.WriteStartArray();
        
        foreach (var operation in operations)
        {
            writer.WriteValue(operation.ColumnName);
        }
        
        writer.WriteEndArray();
        
        writer.WriteEndObject();
    }
    
    private void WriteOperations(JsonWriter writer, IList<IAlterColumnMigrationOperation> operations, JsonSerializer serializer)
    {
        foreach (var operation in operations)
        {
            writer.WriteStartObject();
            
            writer.WritePropertyName("Type");
            writer.WriteValue(operation.OperationType.ToString());
            
            writer.WritePropertyName(nameof(operation.ColumnName));
            writer.WriteValue(operation.ColumnName);
            
            switch (operation.OperationType)
            {
                case ColumnOperationType.ChangeNullability:
                    WriteOperation(writer, (operation as ChangeNullabilityColumnOperation)!);
                    break;
                case ColumnOperationType.RenameColumn:
                    WriteOperation(writer, (operation as RenameColumnOperation)!);
                    break;
                case ColumnOperationType.ChangeDataType:
                    WriteOperation(writer, (operation as ChangeTypeColumnOperation)!);
                    break;
                case ColumnOperationType.ChangeMaxLength:
                    WriteOperation(writer, (operation as ChangeLengthColumnOperation)!);
                    break;
                case ColumnOperationType.ChangePrimaryKey:
                    WriteOperation(writer, (operation as ChangePrimaryKeyColumnOperation)!);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation.OperationType), operation.OperationType,
                        "Can not convert the provided operation type.");
            }

            writer.WriteEndObject();
        }
    }

    private void WriteOperation(JsonWriter writer, ChangeNullabilityColumnOperation operation)
    {
        writer.WritePropertyName(nameof(operation.IsNullable));
        writer.WriteValue(operation.IsNullable);
    }
    
    private void WriteOperation(JsonWriter writer, RenameColumnOperation operation)
    {
        writer.WritePropertyName(nameof(operation.NewColumnName));
        writer.WriteValue(operation.NewColumnName);
    }
    
    private void WriteOperation(JsonWriter writer, ChangeTypeColumnOperation operation)
    {
        writer.WritePropertyName(nameof(operation.NewType));
        writer.WriteValue(operation.NewType.ToString());
    }
    
    private void WriteOperation(JsonWriter writer, ChangeLengthColumnOperation operation)
    {
        writer.WritePropertyName(nameof(operation.ColumnType));
        writer.WriteValue(operation.ColumnType.ToString());
        writer.WritePropertyName(nameof(operation.Length));
        writer.WriteValue(operation.Length ?? null);
    }
    
    private void WriteOperation(JsonWriter writer, ChangePrimaryKeyColumnOperation operation)
    {
        writer.WritePropertyName(nameof(operation.IsPrimaryKey));
        writer.WriteValue(operation.IsPrimaryKey);
    }

    public override ColumnsOperationsCollection ReadJson(JsonReader reader, Type objectType, ColumnsOperationsCollection? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
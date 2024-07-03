using Newtonsoft.Json;
using OrmLibrary.Enums;
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

    private static void WriteOperation(JsonWriter writer, ChangeNullabilityColumnOperation operation)
    {
        writer.WritePropertyName(nameof(operation.IsNullable));
        writer.WriteValue(operation.IsNullable);
        writer.WritePropertyName(nameof(operation.ColumnType));
        writer.WriteValue(operation.ColumnType.ToString());
    }
    
    private static void WriteOperation(JsonWriter writer, RenameColumnOperation operation)
    {
        writer.WritePropertyName(nameof(operation.NewColumnName));
        writer.WriteValue(operation.NewColumnName);
    }
    
    private static void WriteOperation(JsonWriter writer, ChangeTypeColumnOperation operation)
    {
        writer.WritePropertyName(nameof(operation.NewType));
        writer.WriteValue(operation.NewType.ToString());
    }
    
    private static void WriteOperation(JsonWriter writer, ChangeLengthColumnOperation operation)
    {
        writer.WritePropertyName(nameof(operation.ColumnType));
        writer.WriteValue(operation.ColumnType.ToString());
        writer.WritePropertyName(nameof(operation.Length));
        writer.WriteValue(operation.Length ?? null);
    }
    
    private static void WriteOperation(JsonWriter writer, ChangePrimaryKeyColumnOperation operation)
    {
        writer.WritePropertyName(nameof(operation.IsPrimaryKey));
        writer.WriteValue(operation.IsPrimaryKey);
    }

    public override ColumnsOperationsCollection ReadJson(JsonReader reader, Type objectType,
        ColumnsOperationsCollection? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var collection = new ColumnsOperationsCollection();

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
                var operationType = Enum.Parse<ColumnOperationType>(reader.Value.ToString()!);

                switch (operationType)
                {
                    case ColumnOperationType.AddColumn:
                        collection.AddRange(ReadAddColumnOperations(reader, serializer));
                        break;
                    case ColumnOperationType.DropColumn:
                        collection.AddRange(ReadDropColumnOperations(reader, serializer));
                        break;
                    case ColumnOperationType.ChangeMaxLength:
                        collection.Add(ReadChangeMaxLengthOperation(reader));
                        break;
                    case ColumnOperationType.ChangeNullability:
                        collection.Add(ReadChangeNullabilityOperation(reader));
                        break;
                    case ColumnOperationType.ChangePrimaryKey:
                        collection.Add(ReadChangePrimaryKeyOperation(reader));
                        break;
                    case ColumnOperationType.ChangeDataType:
                        collection.Add(ReadChangeDataTypeOperation(reader));
                        break;
                    case ColumnOperationType.RenameColumn:
                        collection.Add(ReadRenameColumnOperation(reader));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(operationType),
                            $"Invalid column operation type provided: {operationType}");
                }
            }
        }

        return collection;
    }

    private static IList<IAddColumnMigrationOperation> ReadAddColumnOperations(JsonReader reader, JsonSerializer serializer)
    {
        var addColumnOperations = new List<IAddColumnMigrationOperation>();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            if (propertyName != "Columns")
            {
                throw new JsonSerializationException(
                    $"Invalid property name provided for the {ColumnOperationType.AddColumn} type.");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray) break;
                var columnProps = serializer.Deserialize<ColumnProperties>(reader)!;
                addColumnOperations.Add(new AddColumnOperation
                {
                    OperationType = ColumnOperationType.AddColumn,
                    NewColumnProps = columnProps
                });
            }
        }

        return addColumnOperations;
    }
    
    private static IList<IDropColumnMigrationOperation> ReadDropColumnOperations(JsonReader reader, JsonSerializer serializer)
    {
        var dropColumnOperations = new List<IDropColumnMigrationOperation>();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            if (propertyName != "Columns")
            {
                throw new JsonSerializationException(
                    $"Invalid property name provided for the {ColumnOperationType.DropColumn} type.");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartArray) continue;
                if (reader.TokenType == JsonToken.EndArray) break;
                
                dropColumnOperations.Add(new DropColumnOperation
                {
                    OperationType = ColumnOperationType.DropColumn,
                    ColumnName = reader.Value.ToString()!
                });
            }
        }

        return dropColumnOperations;
    }

    private static ChangeNullabilityColumnOperation ReadChangeNullabilityOperation(JsonReader reader)
    {
        var operation = new ChangeNullabilityColumnOperation
        {
            OperationType = ColumnOperationType.ChangeNullability
        };
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(ChangeNullabilityColumnOperation.ColumnName):
                    operation.ColumnName = reader.Value.ToString()!;
                    break;
                case nameof(ChangeNullabilityColumnOperation.IsNullable):
                    operation.IsNullable = bool.Parse(reader.Value.ToString()!);
                    break;
                case nameof(ChangeNullabilityColumnOperation.ColumnType):
                    operation.ColumnType = Enum.Parse<SqlType>(reader.Value.ToString()!);
                    break;
            }
        }

        return operation;
    }
    
    private static RenameColumnOperation ReadRenameColumnOperation(JsonReader reader)
    {
        var operation = new RenameColumnOperation
        {
            OperationType = ColumnOperationType.RenameColumn
        };
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(RenameColumnOperation.ColumnName):
                    operation.ColumnName = reader.Value.ToString()!;
                    break;
                case nameof(RenameColumnOperation.NewColumnName):
                    operation.NewColumnName = reader.Value.ToString()!;
                    break;
            }
        }

        return operation;
    }
    
    private static ChangeTypeColumnOperation ReadChangeDataTypeOperation(JsonReader reader)
    {
        var operation = new ChangeTypeColumnOperation
        {
            OperationType = ColumnOperationType.ChangeDataType
        };
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(ChangeTypeColumnOperation.ColumnName):
                    operation.ColumnName = reader.Value.ToString()!;
                    break;
                case nameof(ChangeTypeColumnOperation.NewType):
                    operation.NewType = Enum.Parse<SqlType>(reader.Value.ToString()!);
                    break;
            }
        }

        return operation;
    }
    
    private static ChangeLengthColumnOperation ReadChangeMaxLengthOperation(JsonReader reader)
    {
        var operation = new ChangeLengthColumnOperation
        {
            OperationType = ColumnOperationType.ChangeMaxLength
        };
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(ChangeLengthColumnOperation.ColumnName):
                    operation.ColumnName = reader.Value.ToString()!;
                    break;
                case nameof(ChangeLengthColumnOperation.ColumnType):
                    operation.ColumnType = Enum.Parse<SqlType>(reader.Value.ToString()!);
                    break;
                case nameof(ChangeLengthColumnOperation.Length):
                    operation.Length = int.TryParse(reader.Value?.ToString(), out var length) ? length : null;
                    break;
            }
        }

        return operation;
    }
    
    private static ChangePrimaryKeyColumnOperation ReadChangePrimaryKeyOperation(JsonReader reader)
    {
        var operation = new ChangePrimaryKeyColumnOperation
        {
            OperationType = ColumnOperationType.ChangePrimaryKey
        };
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            if (reader.TokenType != JsonToken.PropertyName) continue;
            
            var propertyName = reader.Value!.ToString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(ChangePrimaryKeyColumnOperation.ColumnName):
                    operation.ColumnName = reader.Value.ToString()!;
                    break;
                case nameof(ChangePrimaryKeyColumnOperation.IsPrimaryKey):
                    operation.IsPrimaryKey = bool.Parse(reader.Value.ToString()!);
                    break;
            }
        }

        return operation;
    }
}
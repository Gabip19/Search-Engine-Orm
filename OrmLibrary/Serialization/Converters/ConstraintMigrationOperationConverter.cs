using Newtonsoft.Json;
using OrmLibrary.Enums;
using OrmLibrary.Migrations.MigrationOperations.Tables;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

namespace OrmLibrary.Serialization.Converters;

public class ConstraintMigrationOperationConverter : JsonConverter<IConstraintMigrationOperation>
{
    public override void WriteJson(JsonWriter writer, IConstraintMigrationOperation? value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        
        writer.WritePropertyName("Type");
        writer.WriteValue(value.OperationType.ToString());
        
        writer.WritePropertyName(nameof(value.TableName));
        writer.WriteValue(value.TableName);
        writer.WritePropertyName(nameof(value.ConstraintName));
        writer.WriteValue(value.ConstraintName);
        
        switch (value.OperationType)
        {
            case TableOperationType.AddConstraint:
                WriteOperation(writer, (value as AddConstraintOperation)!, serializer);
                break;
            case TableOperationType.DropConstraint:
                break;
            case TableOperationType.AlterForeignKey:
                WriteOperation(writer, (value as AlterForeignKeyConstraintOperation)!, serializer);
                break;
            case TableOperationType.AlterPrimaryKey:
                WriteOperation(writer, (value as AlterPrimaryKeyConstraintOperation)!);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value.OperationType), value.OperationType,
                    "Can not convert the provided operation type.");
        }

        writer.WriteEndObject();
    }

    private void WriteOperation(JsonWriter writer, IAddConstraintMigrationOperation operation, JsonSerializer serializer)
    {
        writer.WritePropertyName("constraintType");
        writer.WriteValue(operation.ConstraintType.ToString());
        
        switch (operation.ConstraintType)
        {
            case TableConstraintType.ForeignKeyConstraint:
                WriteOperation(writer, (operation as AddForeignKeyConstraintOperation)!, serializer);
                break;
            case TableConstraintType.UniqueConstraint:
                WriteOperation(writer, (operation as AddUniqueConstraintOperation)!, serializer);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation.ConstraintType),
                    "Invalid constraint type provided");
        }
    }

    private void WriteOperation(JsonWriter writer, AddForeignKeyConstraintOperation operation, JsonSerializer serializer)
    {
        writer.WritePropertyName("foreignKeyGroup");
        serializer.Serialize(writer, operation.ForeignKeyGroupDto, typeof(ForeignKeyGroupDto));
    }
    
    private void WriteOperation(JsonWriter writer, AddUniqueConstraintOperation operation, JsonSerializer serializer)
    {
        writer.WritePropertyName("columnName");
        writer.WriteValue(operation.ColumnName);
    }
    
    private void WriteOperation(JsonWriter writer, AlterForeignKeyConstraintOperation operation, JsonSerializer serializer)
    {
        writer.WritePropertyName("foreignKeyGroup");
        serializer.Serialize(writer, operation.KeyGroupDto, typeof(ForeignKeyGroupDto));
    }
    
    private void WriteOperation(JsonWriter writer, AlterPrimaryKeyConstraintOperation operation)
    {
        writer.WritePropertyName(nameof(operation.PrimaryKeyColumns));
        writer.WriteStartArray();
        foreach (var pkColumnName in operation.PrimaryKeyColumns)
        {
            writer.WriteValue(pkColumnName);
        }
        writer.WriteEndArray();
    }

    public override IConstraintMigrationOperation ReadJson(JsonReader reader, Type objectType,
        IConstraintMigrationOperation? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string tableName = null!;
        string constraintName = null!;
        TableOperationType? operationType = hasExistingValue && existingValue is not null
            ? existingValue.OperationType
            : null;
        
        TableConstraintType? constraintType = null;
        ForeignKeyGroupDto? foreignKeyGroupDto = null;
        string? columnName = null;
        List<string>? primaryKeyColumns = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value!.ToString();
                reader.Read();

                switch (propertyName)
                {
                    case "Type":
                        operationType = hasExistingValue && existingValue is not null
                            ? existingValue.OperationType
                            : Enum.Parse<TableOperationType>(reader.Value.ToString()!);
                        break;
                    case nameof(IConstraintMigrationOperation.TableName):
                        tableName = reader.Value.ToString()!;
                        break;
                    case nameof(IConstraintMigrationOperation.ConstraintName):
                        constraintName = reader.Value.ToString()!;
                        break;
                    case "constraintType":
                        constraintType = Enum.Parse<TableConstraintType>(reader.Value.ToString()!);
                        break;
                    case "foreignKeyGroup":
                        foreignKeyGroupDto = serializer.Deserialize<ForeignKeyGroupDto>(reader);
                        break;
                    case "columnName":
                        columnName = reader.Value.ToString();
                        break;
                    case nameof(AlterPrimaryKeyConstraintOperation.PrimaryKeyColumns):
                        primaryKeyColumns = new List<string>();
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                        {
                            primaryKeyColumns.Add(reader.Value.ToString()!);
                        }
                        break;
                }
            }
        }

        if (operationType == null)
        {
            throw new JsonSerializationException("Missing Type property");
        }

        return operationType switch
        {
            TableOperationType.AddConstraint => constraintType switch
            {
                TableConstraintType.ForeignKeyConstraint => new AddForeignKeyConstraintOperation(
                    tableName,
                    operationType.Value,
                    constraintName,
                    constraintType.Value
                )
                {
                    ForeignKeyGroupDto = foreignKeyGroupDto!
                },
                TableConstraintType.UniqueConstraint => new AddUniqueConstraintOperation(
                    tableName,
                    operationType.Value,
                    constraintName,
                    constraintType.Value
                )
                {
                    ColumnName = columnName!
                },
                _ => throw new JsonSerializationException($"Unsupported constraint type: {constraintType}")
            },
            TableOperationType.DropConstraint => new DropConstraintOperation
            {
                TableName = tableName,
                ConstraintName = constraintName,
                OperationType = operationType.Value
            },
            TableOperationType.AlterForeignKey => new AlterForeignKeyConstraintOperation
            {
                TableName = tableName,
                ConstraintName = constraintName,
                KeyGroupDto = foreignKeyGroupDto!,
                OperationType = operationType.Value
            },
            TableOperationType.AlterPrimaryKey => new AlterPrimaryKeyConstraintOperation
            {
                TableName = tableName,
                ConstraintName = constraintName,
                PrimaryKeyColumns = primaryKeyColumns!,
                OperationType = operationType.Value
            },
            _ => throw new JsonSerializationException($"Unsupported operation type: {operationType}")
        };
    }
}
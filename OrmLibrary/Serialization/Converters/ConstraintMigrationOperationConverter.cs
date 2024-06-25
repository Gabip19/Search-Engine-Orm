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

    public override IConstraintMigrationOperation ReadJson(JsonReader reader, Type objectType, IConstraintMigrationOperation? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
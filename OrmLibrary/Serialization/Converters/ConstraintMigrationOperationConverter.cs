using Newtonsoft.Json;
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
                // WriteOperation(writer, (value as )!);
                break;
            case TableOperationType.DropConstraint:
                break;
            case TableOperationType.AlterForeignKey:
                // WriteOperation(writer, (operation as ChangeTypeColumnOperation)!);
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

    // private void WriteOperation(JsonWriter writer,  operation)
    // {
    //     writer.WritePropertyName(nameof(operation.ColumnName));
    //     writer.WriteValue(operation.ColumnName);
    //     writer.WritePropertyName(nameof(operation.IsNullable));
    //     writer.WriteValue(operation.IsNullable);
    // }
    
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
    
    // private void WriteOperation(JsonWriter writer,  operation)
    // {
    //     writer.WritePropertyName(nameof(operation.ColumnName));
    //     writer.WriteValue(operation.ColumnName);
    //     writer.WritePropertyName(nameof(operation.ColumnType));
    //     writer.WriteValue(operation.ColumnType.ToString());
    //     writer.WritePropertyName(nameof(operation.Length));
    //     writer.WriteValue(operation.Length ?? null);
    // }

    public override IConstraintMigrationOperation ReadJson(JsonReader reader, Type objectType, IConstraintMigrationOperation? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
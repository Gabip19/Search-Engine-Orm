using OrmLibrary.Enums;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

namespace OrmLibrary.Migrations.MigrationOperations.Columns;

public static class ColumnMigrationOperationsFactory
{
    public static RenameColumnOperation NewRenameOperation(ColumnProperties column, string newColumnName) =>
        new(column.TableName, column.Name, "rename")
        {
            NewColumnName = newColumnName
        };

    public static ChangeTypeColumnOperation NewChangeTypeOperation(ColumnProperties column, SqlType newType) =>
        new(column.TableName, column.Name, "change_data_type")
        {
            NewType = newType
        };

    public static ChangeNullabilityColumnOperation NewChangeNullabilityOperation(ColumnProperties column,
        bool isNullable) => new(column.TableName, column.Name, "change_nullability")
        {
            ColumnType = column.SqlColumnType,
            IsNullable = isNullable
        };

    public static ChangeLengthColumnOperation NewChangeLengthOperation(ColumnProperties column, SqlType columnType,
        int? newLength) => new(column.TableName, column.Name, "change_max_length")
        {
            ColumnType = columnType,
            Length = newLength
        };

    public static AddColumnMigrationOperation NewAddColumnOperation(ColumnProperties newColumnProps)
        => new()
        {
            TableName = newColumnProps.TableName,
            OperationType = "add_column",
            NewColumnProps = newColumnProps
        };

    public static DropColumnMigrationOperation NewDropColumnOperation(ColumnProperties droppedColumn) =>
        new()
        {
            TableName = droppedColumn.TableName,
            OperationType = "drop_column",
            ColumnName = droppedColumn.Name
        };
}
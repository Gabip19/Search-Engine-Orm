using OrmLibrary.Enums;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

namespace OrmLibrary.Migrations.MigrationOperations.Columns;

public static class ColumnMigrationOperationsFactory
{
    public static RenameColumnOperation NewRenameOperation(ColumnProperties column, string newColumnName) =>
        new(column.TableName, column.Name, ColumnOperationType.RenameColumn)
        {
            NewColumnName = newColumnName
        };

    public static ChangeTypeColumnOperation NewChangeTypeOperation(ColumnProperties column, SqlType newType) =>
        new(column.TableName, column.Name, ColumnOperationType.ChangeDataType)
        {
            NewType = newType
        };

    public static ChangeNullabilityColumnOperation NewChangeNullabilityOperation(ColumnProperties column,
        bool isNullable) => new(column.TableName, column.Name, ColumnOperationType.ChangeNullability)
        {
            ColumnType = column.SqlColumnType,
            IsNullable = isNullable
        };

    public static ChangeLengthColumnOperation NewChangeLengthOperation(ColumnProperties column, SqlType columnType,
        int? newLength) => new(column.TableName, column.Name, ColumnOperationType.ChangeMaxLength)
        {
            ColumnType = columnType,
            Length = newLength
        };

    public static AddColumnOperation NewAddColumnOperation(ColumnProperties newColumnProps)
        => new()
        {
            TableName = newColumnProps.TableName,
            OperationType = ColumnOperationType.AddColumn,
            NewColumnProps = newColumnProps
        };

    public static DropColumnOperation NewDropColumnOperation(ColumnProperties droppedColumn) =>
        new()
        {
            TableName = droppedColumn.TableName,
            OperationType = ColumnOperationType.DropColumn,
            ColumnName = droppedColumn.Name
        };

    public static ChangePrimaryKeyColumnOperation NewPrimaryKeyChangeOperation(ColumnProperties column,
        bool isPrimaryKey) => new(column.TableName, column.Name, ColumnOperationType.ChangePrimaryKey)
    {
        IsPrimaryKey = isPrimaryKey
    };
}
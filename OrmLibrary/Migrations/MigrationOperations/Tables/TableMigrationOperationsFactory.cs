using OrmLibrary.Constraints;
using OrmLibrary.Extensions;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

namespace OrmLibrary.Migrations.MigrationOperations.Tables;

public static class TableMigrationOperationsFactory
{
    public static AlterForeignKeyConstraintOperation NewAlterForeignKeyOperation(ForeignKeyConstraint newConstraint) =>
        new()
        {
            TableName = newConstraint.TableName,
            OperationType = TableOperationType.AlterForeignKey,
            ConstraintName = newConstraint.Name,
            KeyGroupDto = newConstraint.ForeignKeyGroup.MapToDto()
        };
    
    public static DropConstraintOperation NewDropConstraintOperation(ITableConstraint droppedConstraint) =>
        new()
        {
            TableName = droppedConstraint.TableName,
            OperationType = TableOperationType.DropConstraint,
            ConstraintName = droppedConstraint.Name
        };
    
    public static AddConstraintOperation NewAddConstraintOperation(ITableConstraint newConstraint) =>
        new()
        {
            TableName = newConstraint.TableName,
            OperationType = TableOperationType.AddConstraint,
            ConstraintName = newConstraint.Name,
            NewConstraint = newConstraint
        };

    public static AlterTableStructureOperation NewAlterTableStructureOperation(string tableName,
        IList<IColumnMigrationOperation> columnOperations) => new()
        {
            TableName = tableName,
            OperationType = TableOperationType.AlterTable,
            ColumnOperations = columnOperations
        };

    public static AlterPrimaryKeyConstraintOperation NewAlterPrimaryKeyOperation(TableProperties tableProps,
        IList<ColumnProperties> newPrimaryKeys) => new()
    {
        TableName = tableProps.Name,
        OperationType = TableOperationType.AlterPrimaryKey,
        ConstraintName = $"PK_{tableProps.Name}",
        PrimaryKeyColumns = newPrimaryKeys.Select(column => column.Name).ToList()
    };
    
    public static AddTableOperation NewAddTableOperation(TableProperties newTableProps)
        => new()
        {
            TableName = newTableProps.Name,
            OperationType = TableOperationType.AddTable,
            NewTableProps = newTableProps
        };

    public static DropTableOperation NewDropTableOperation(TableProperties droppedTable) =>
        new()
        {
            TableName = droppedTable.Name,
            OperationType = TableOperationType.DropTable
        };
}
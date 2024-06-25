using OrmLibrary.Constraints;
using OrmLibrary.Enums;
using OrmLibrary.Extensions;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
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
        newConstraint switch
        {
            ForeignKeyConstraint foreignKeyConstraint => NewAddForeignKeyConstraintOperation(foreignKeyConstraint),
            UniqueConstraint uniqueConstraint => NewAddUniqueConstraintOperation(uniqueConstraint),
            _ => throw new ArgumentOutOfRangeException(nameof(newConstraint), "Invalid constraint type provided")
        };

    private static AddForeignKeyConstraintOperation NewAddForeignKeyConstraintOperation(
        ForeignKeyConstraint foreignKeyConstraint) =>
        new(foreignKeyConstraint.TableName, TableOperationType.AddConstraint, foreignKeyConstraint.Name,
            TableConstraintType.ForeignKeyConstraint)
        {
            ForeignKeyGroupDto = foreignKeyConstraint.ForeignKeyGroup.MapToDto()
        };

    private static AddUniqueConstraintOperation NewAddUniqueConstraintOperation(UniqueConstraint uniqueConstraint) =>
        new(uniqueConstraint.TableName, TableOperationType.AddConstraint, uniqueConstraint.Name,
            TableConstraintType.UniqueConstraint)
        {
            ColumnName = uniqueConstraint.ColumnName
        };

    public static AlterTableStructureOperation NewAlterTableStructureOperation(string tableName,
        ColumnsOperationsCollection columnOperations) => new()
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
    
    public static AddTableOperation NewAddTableOperation(TableProperties newTableProps) => 
        new()
        {
            TableName = newTableProps.Name,
            OperationType = TableOperationType.AddTable,
            Columns = newTableProps.Columns.ToList()
        };

    public static DropTableOperation NewDropTableOperation(TableProperties droppedTable) =>
        new()
        {
            TableName = droppedTable.Name,
            OperationType = TableOperationType.DropTable
        };
}
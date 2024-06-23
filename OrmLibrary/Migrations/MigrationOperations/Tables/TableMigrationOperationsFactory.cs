﻿using OrmLibrary.Constraints;
using OrmLibrary.Extensions;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

namespace OrmLibrary.Migrations.MigrationOperations.Tables;

public static class TableMigrationOperationsFactory
{
    public static AlterForeignKeyConstraintOperation NewAlterForeignKeyOperation(ForeignKeyConstraint newConstraint) =>
        new()
        {
            TableName = newConstraint.TableName,
            OperationType = "alter_foreign_key",
            ConstraintName = newConstraint.Name,
            KeyGroupDto = newConstraint.ForeignKeyGroup.MapToDto()
        };
    
    public static DropConstraintOperation NewDropConstraintOperation(ITableConstraint droppedConstraint) =>
        new()
        {
            TableName = droppedConstraint.TableName,
            OperationType = "drop_constraint",
            ConstraintName = droppedConstraint.Name
        };
    
    public static AddConstraintOperation NewAddConstraintOperation(ITableConstraint newConstraint) =>
        new()
        {
            TableName = newConstraint.TableName,
            OperationType = "add_constraint",
            ConstraintName = newConstraint.Name,
            NewConstraint = newConstraint
        };

    public static AlterTableStructureOperation NewAlterTableStructureOperation(string tableName,
        IList<IColumnMigrationOperation> columnOperations) => new()
        {
            TableName = tableName,
            OperationType = "alter_table",
            ColumnOperations = columnOperations
        };
}
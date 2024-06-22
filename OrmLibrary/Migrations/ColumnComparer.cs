using OrmLibrary.Mappings;
using OperationsFactory = OrmLibrary.Migrations.MigrationOperations.Columns.ColumnMigrationOperationsFactory;

namespace OrmLibrary.Migrations;

// TODO: column comparer
/*
 * alter name - Name
 * alter type - SqlColumnType
 * alter nullable - IsNullable
 * alter fixed length ? - IsFixedLength
 * alter max length - MaxLength
 * alter precision - Precision
 */

public class ColumnComparer
{
    public ColumnDifference CompareColumns(ColumnProperties oldColumn, ColumnProperties newColumn)
    {
        var columnDifference = new ColumnDifference();

        // Check for column name change
        if (oldColumn.Name != newColumn.Name)
        {
            columnDifference.Operations.Add(OperationsFactory.NewRenameOperation(oldColumn, newColumn.Name));
        }

        // Check for data type change
        if (oldColumn.SqlColumnType != newColumn.SqlColumnType)
        {
            columnDifference.Operations.Add(OperationsFactory.NewChangeTypeOperation(oldColumn, newColumn.SqlColumnType));
        }

        // Check for nullability change
        if (oldColumn.IsNullable != newColumn.IsNullable)
        {
            columnDifference.Operations.Add(OperationsFactory.NewChangeNullabilityOperation(oldColumn, newColumn.IsNullable));
        }

        // Check for max length change
        if (oldColumn.MaxLength != newColumn.MaxLength)
        {
            columnDifference.Operations.Add(OperationsFactory.NewChangeLengthOperation(oldColumn, newColumn.SqlColumnType, newColumn.MaxLength));
        }

        // Check for default value change
        // if (oldColumn.DefaultValue != newColumn.DefaultValue)
        // {
        //     columnDifference.Operations.Add(new ColumnMigrationOperation("change_default_value", oldColumn.TableName, oldColumn, newColumn));
        // }

        // Check for computed column SQL change
        // if (oldColumn.ComputedColumnSql != newColumn.ComputedColumnSql)
        // {
        //     columnDifference.Operations.Add(new ColumnMigrationOperation("change_computed_column_sql", oldColumn.TableName, oldColumn, newColumn));
        // }

        return columnDifference;
    }
}


using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations;

namespace OrmLibrary.Migrations;

public class ColumnComparer
{
    public ColumnDifference CompareColumns(ColumnProperties oldColumn, ColumnProperties newColumn)
    {
        var columnDifference = new ColumnDifference();

        // Check for column name change
        if (oldColumn.Name != newColumn.Name)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("rename", oldColumn.TableName, oldColumn, newColumn));
        }

        // Check for data type change
        if (oldColumn.SqlColumnType != newColumn.SqlColumnType)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("change_data_type", oldColumn.TableName, oldColumn, newColumn));
        }

        // Check for nullability change
        if (oldColumn.IsNullable != newColumn.IsNullable)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("change_nullability", oldColumn.TableName, oldColumn, newColumn));
        }

        // Check for primary key change
        if (oldColumn.IsPrimaryKeyColumn != newColumn.IsPrimaryKeyColumn)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("change_primary_key", oldColumn.TableName, oldColumn, newColumn));
        }

        // Check for foreign key change
        if (oldColumn.IsForeignKeyColumn != newColumn.IsForeignKeyColumn)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("change_foreign_key", oldColumn.TableName, oldColumn, newColumn));
        }

        // Check for uniqueness change
        if (oldColumn.IsUnique != newColumn.IsUnique)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("change_uniqueness", oldColumn.TableName, oldColumn, newColumn));
        }

        // Check for fixed length change
        if (oldColumn.IsFixedLength != newColumn.IsFixedLength)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("change_fixed_length", oldColumn.TableName, oldColumn, newColumn));
        }

        // Check for max length change
        if (oldColumn.MaxLength != newColumn.MaxLength)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("change_max_length", oldColumn.TableName, oldColumn, newColumn));
        }

        // Check for precision change
        if (oldColumn.Precision != newColumn.Precision)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("change_precision", oldColumn.TableName, oldColumn, newColumn));
        }

        // Check for default value change
        if (oldColumn.DefaultValue != newColumn.DefaultValue)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("change_default_value", oldColumn.TableName, oldColumn, newColumn));
        }

        // Check for computed column SQL change
        if (oldColumn.ComputedColumnSql != newColumn.ComputedColumnSql)
        {
            columnDifference.Operations.Add(new ColumnMigrationOperation("change_computed_column_sql", oldColumn.TableName, oldColumn, newColumn));
        }

        return columnDifference;
    }
}


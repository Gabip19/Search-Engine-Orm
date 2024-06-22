using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables;
using OperationsFactory = OrmLibrary.Migrations.MigrationOperations.Columns.ColumnMigrationOperationsFactory;

namespace OrmLibrary.Migrations;

// TODO: table comparer
/*
 * alter primary key - PrimaryKey Checked
 * alter foreign key - Constraints Checked?
 * alter unique - Drop constraint based check
 */

public class TableComparer
{
    private readonly ColumnComparer _columnComparer = new();

    public List<TableMigrationOperation> CompareTables(TableProperties lastState, TableProperties currentState)
    {
        var columnOperations = GetColumnsOperations(lastState, currentState);
    }

    private static bool TryGetMatchingColumn(TableProperties tableProps, ColumnProperties columnProps, out ColumnProperties? matchedColumn)
    {
        return tableProps.TryGetColumnInfo(columnProps.Name, out matchedColumn) ||
               (columnProps.PropertyName is not null &&
                tableProps.TryGetColumnInfoByProperty(columnProps.PropertyName, out matchedColumn));
    }

    private List<IColumnMigrationOperation> GetColumnsOperations(TableProperties lastState, TableProperties currentState)
    {
        var unmappedPreviousStateColumns = lastState.Columns.ToHashSet();
        
        var columnsOperations = new List<IColumnMigrationOperation>();
        
        foreach (var currentStateColumn in currentState.Columns)
        {
            if (TryGetMatchingColumn(lastState, currentStateColumn, out var lastStateColumn))
            {
                // matched by name / matched by mapped property - check differences
                unmappedPreviousStateColumns.Remove(lastStateColumn!);
                
                var columnDifference = _columnComparer.CompareColumns(lastStateColumn!, currentStateColumn);
                if (columnDifference.Operations.Any())
                {
                    columnsOperations.AddRange(columnDifference.Operations);
                }
            }
            else
            {
                // column added
                columnsOperations.Add(OperationsFactory.NewAddColumnOperation(currentStateColumn));
            }
        }

        // dropped columns
        columnsOperations.AddRange(unmappedPreviousStateColumns.Select(OperationsFactory.NewDropColumnOperation));

        return columnsOperations;
    }
}

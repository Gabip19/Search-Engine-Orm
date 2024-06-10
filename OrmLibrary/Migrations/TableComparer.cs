using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations;

namespace OrmLibrary.Migrations;

public class TableComparer
{
    private readonly ColumnComparer _columnComparer = new();

    public List<TableMigrationOperation> CompareTables(TableProperties lastState, TableProperties currentState)
    {
        var unmappedPreviousStateColumns = lastState.Columns.ToHashSet();
        
        var operations = new List<TableMigrationOperation>();

        if (lastState.Name != currentState.Name)
        {
            operations.Add(new TableMigrationOperation("rename", lastState.Name, currentState.Name));
        }
        
        foreach (var currentStateColumn in currentState.Columns)
        {
            if (lastState.TryGetColumnInfo(currentStateColumn.Name, out var lastStateColumn) || 
                lastState.TryGetColumnInfoByProperty(currentStateColumn.PropertyName, out lastStateColumn))
            {
                // matched by name / matched by mapped property
                unmappedPreviousStateColumns.Remove(lastStateColumn!);
                
                var columnDifference = _columnComparer.CompareColumns(lastStateColumn, currentStateColumn);
                if (columnDifference.Operations.Any())
                {
                    var tableOp = new TableMigrationOperation("modify_columns", lastState.Name);
                    tableOp.ColumnOperations.AddRange(columnDifference.Operations);
                    operations.Add(tableOp);
                }
            }
            else
            {
                // column added
                var tableOp = new TableMigrationOperation("add_columns", currentStateColumn.Name);
                tableOp.ColumnOperations.Add(new ColumnMigrationOperation("add", currentStateColumn.Name, currentStateColumn));
                operations.Add(tableOp);
            }
        }

        foreach (var previousStateColumn in unmappedPreviousStateColumns)
        {
            // column dropped
            var tableOp = new TableMigrationOperation("drop_columns", previousStateColumn.Name);
            tableOp.ColumnOperations.Add(new ColumnMigrationOperation("drop", previousStateColumn.Name, previousStateColumn));
            operations.Add(tableOp);
        }

        return operations;
    }
}

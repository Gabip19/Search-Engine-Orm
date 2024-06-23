using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using ColumnOperationsFactory = OrmLibrary.Migrations.MigrationOperations.Columns.ColumnMigrationOperationsFactory;
using TableOperationsFactory = OrmLibrary.Migrations.MigrationOperations.Tables.TableMigrationOperationsFactory;

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
    private readonly ConstraintComparer _constraintComparer = new();

    public List<ITableMigrationOperation> CompareTables(TableProperties lastState, TableProperties currentState)
    {
        var tableOperations = new List<ITableMigrationOperation>();
        
        var columnOperations = GetColumnsOperations(lastState, currentState);

        if (columnOperations.Any())
        {
            tableOperations.Add(
                TableOperationsFactory.NewAlterTableStructureOperation(currentState.Name, columnOperations));
        }
        
        tableOperations.AddRange(GetConstraintsOperations(lastState,  currentState));

        return tableOperations;
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
                columnsOperations.Add(ColumnOperationsFactory.NewAddColumnOperation(currentStateColumn));
            }
        }

        // dropped columns
        columnsOperations.AddRange(unmappedPreviousStateColumns.Select(ColumnOperationsFactory.NewDropColumnOperation));

        return columnsOperations;
    }
    
    private List<IConstraintMigrationOperation> GetConstraintsOperations(TableProperties lastState, TableProperties currentState)
    {
        var notFoundCurrentStateConstraints = currentState.Constraints.ToDictionary(constraint => constraint.Name);

        var constraintOperations = new List<IConstraintMigrationOperation>();

        foreach (var lastStateConstraint in lastState.Constraints)
        {
            if (notFoundCurrentStateConstraints.TryGetValue(lastStateConstraint.Name, out var currentStateConstraint))
            {
                notFoundCurrentStateConstraints.Remove(currentStateConstraint.Name);

                var constraintOperation =
                    _constraintComparer.CompareConstraints(lastStateConstraint, currentStateConstraint);

                if (constraintOperation != null)
                {
                    constraintOperations.Add(constraintOperation);
                }
            }
            else
            {
                constraintOperations.Add(TableOperationsFactory.NewDropConstraintOperation(lastStateConstraint));
            }
        }

        constraintOperations.AddRange(notFoundCurrentStateConstraints.Values.Select(TableOperationsFactory.NewAddConstraintOperation));

        return constraintOperations;
    }
}

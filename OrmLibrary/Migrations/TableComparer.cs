using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations;
using OrmLibrary.Migrations.MigrationOperations.Columns;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using ColumnOperationsFactory = OrmLibrary.Migrations.MigrationOperations.Columns.ColumnMigrationOperationsFactory;
using TableOperationsFactory = OrmLibrary.Migrations.MigrationOperations.Tables.TableMigrationOperationsFactory;

namespace OrmLibrary.Migrations;

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

        if (columnOperations.AlterColumnOperations.Any(operation => operation.OperationType == ColumnOperationType.ChangePrimaryKey))
        {
            tableOperations.Add(TableOperationsFactory.NewAlterPrimaryKeyOperation(currentState, currentState.PrimaryKeys));
        }

        return tableOperations;
    }

    private static bool TryGetMatchingColumn(TableProperties tableProps, ColumnProperties columnProps, out ColumnProperties? matchedColumn)
    {
        if (tableProps.TryGetColumnInfo(columnProps.Name, out matchedColumn) ||
            (columnProps.PropertyName is not null &&
             tableProps.TryGetColumnInfoByProperty(columnProps.PropertyName, out matchedColumn)))
        {
            return true;
        }

        if (columnProps is not { IsForeignKeyColumn: true, ForeignKeyGroup: not null }) return false;
        
        var columnGroup = columnProps.ForeignKeyGroup;

        var matchedFkGroup = tableProps.ForeignKeys
            .FirstOrDefault(group => group.ReferencedTableName == columnGroup.ReferencedTableName &&
                                     (group.AssociatedPropertyName == columnGroup.AssociatedPropertyName ||
                                      group.ColumnsNamesPrefix == columnGroup.ColumnsNamesPrefix));

        if (matchedFkGroup is null) return false;

        var referencedColumn = columnGroup.KeyPairs.First(pair => pair.MainColumn.Name == columnProps.Name)
            .ReferencedColumn;

        var matchedFkPair = matchedFkGroup.KeyPairs
            .FirstOrDefault(pair => pair.ReferencedColumn.Name == referencedColumn.Name ||
                                    (referencedColumn.PropertyName is not null &&
                                     pair.ReferencedColumn.PropertyName == referencedColumn.PropertyName));

        if (matchedFkPair is null) return false;
        
        matchedColumn = matchedFkPair.MainColumn;
        return true;
    }

    private ColumnsOperationsCollection GetColumnsOperations(TableProperties lastState, TableProperties currentState)
    {
        var unmappedPreviousStateColumns = lastState.Columns.ToHashSet();
        
        var columnsOperations = new ColumnsOperationsCollection();
        
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
    
    // TODO: refactor
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

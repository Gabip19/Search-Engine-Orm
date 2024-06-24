using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations;

public class ColumnsOperationsCollection
{
    public IList<IAddColumnMigrationOperation> AddColumnOperations { get; } =
        new List<IAddColumnMigrationOperation>();

    public IList<IDropColumnMigrationOperation> DropColumnOperations { get; } =
        new List<IDropColumnMigrationOperation>();

    public IList<IAlterColumnMigrationOperation> AlterColumnOperations { get; } =
        new List<IAlterColumnMigrationOperation>();

    public void AddRange(IEnumerable<IColumnMigrationOperation> columnMigrationOperations)
    {
        foreach (var columnOperation in columnMigrationOperations)
        {
            Add(columnOperation);
        }
    }

    public void Add(IColumnMigrationOperation columnOperation)
    {
        switch (columnOperation)
        {
            case IAddColumnMigrationOperation addOperation:
                AddColumnOperations.Add(addOperation);
                break;
            case IDropColumnMigrationOperation dropOperation:
                DropColumnOperations.Add(dropOperation);
                break;
            case IAlterColumnMigrationOperation alterOperation:
                AlterColumnOperations.Add(alterOperation);
                break;
        }
    }

    public bool Any()
    {
        return AddColumnOperations.Any() || DropColumnOperations.Any() || AlterColumnOperations.Any();
    }
}
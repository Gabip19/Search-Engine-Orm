using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations;

public class MigrationOperationsCollection
{
    public IList<IDropTableMigrationOperation> DropTableOperations { get; } =
        new List<IDropTableMigrationOperation>();

    public IList<IAddTableMigrationOperation> AddTableOperations { get; } =
        new List<IAddTableMigrationOperation>();

    public IList<IAlterTableMigrationOperation> AlterTableOperations { get; } =
        new List<IAlterTableMigrationOperation>();
    
    public void AddRange(IEnumerable<ITableMigrationOperation> columnMigrationOperations)
    {
        foreach (var columnOperation in columnMigrationOperations)
        {
            Add(columnOperation);
        }
    }

    public void Add(ITableMigrationOperation columnOperation)
    {
        switch (columnOperation)
        {
            case IAddTableMigrationOperation addOperation:
                AddTableOperations.Add(addOperation);
                break;
            case IDropTableMigrationOperation dropOperation:
                DropTableOperations.Add(dropOperation);
                break;
            case IAlterTableMigrationOperation alterOperation:
                AlterTableOperations.Add(alterOperation);
                break;
        }
    }

    public bool Any()
    {
        return AddTableOperations.Any() || DropTableOperations.Any() || AlterTableOperations.Any();
    }
}
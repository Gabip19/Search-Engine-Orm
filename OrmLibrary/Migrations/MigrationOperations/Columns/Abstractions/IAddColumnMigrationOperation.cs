using OrmLibrary.Mappings;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

public interface IAddColumnMigrationOperation : IColumnMigrationOperation
{
    public ColumnProperties NewColumnProps { get; set; }
}
using OrmLibrary.Mappings;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public interface IAddTableMigrationOperation : ITableMigrationOperation
{
    public TableProperties NewTableProps { get; set; }
}
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

public class DropTableOperation : IDropTableMigrationOperation
{
    public string TableName { get; set; }
    public TableOperationType OperationType { get; set; }
}
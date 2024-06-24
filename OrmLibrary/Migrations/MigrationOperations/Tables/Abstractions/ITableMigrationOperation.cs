namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public interface ITableMigrationOperation : IMigrationOperation
{
    public TableOperationType OperationType { get; set; }
}
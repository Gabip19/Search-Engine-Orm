namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public interface ITableMigrationOperation : IMigrationOperation
{
    public string OperationType { get; set; }
}
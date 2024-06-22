namespace OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

public interface IColumnMigrationOperation : IMigrationOperation
{
    public string OperationType { get; set; }
}
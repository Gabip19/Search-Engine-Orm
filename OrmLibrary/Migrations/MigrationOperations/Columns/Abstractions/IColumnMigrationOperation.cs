namespace OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

public interface IColumnMigrationOperation : IMigrationOperation
{
    public ColumnOperationType OperationType { get; set; }
}
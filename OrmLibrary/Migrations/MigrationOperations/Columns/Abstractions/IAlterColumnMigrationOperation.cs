namespace OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

public interface IAlterColumnMigrationOperation : IColumnMigrationOperation
{
    public string ColumnName { get; set; }
}
namespace OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

public interface IDropColumnMigrationOperation : IColumnMigrationOperation
{
    public string ColumnName { get; set; }
}
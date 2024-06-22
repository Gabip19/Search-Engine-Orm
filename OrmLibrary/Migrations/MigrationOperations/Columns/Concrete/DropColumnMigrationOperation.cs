using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class DropColumnMigrationOperation : IDropColumnMigrationOperation
{
    public string TableName { get; set; }
    public string OperationType { get; set; }
    public string ColumnName { get; set; }
}
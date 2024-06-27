using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class RenameColumnOperation : AlterColumnMigrationOperation
{
    public string NewColumnName { get; set; } = null!;

    public RenameColumnOperation() { }
    
    public RenameColumnOperation(string tableName, string columnName, ColumnOperationType operationType) : base(tableName, columnName, operationType)
    {
    }
}
namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class RenameColumnOperation : AlterColumnMigrationOperation
{
    public string NewColumnName { get; set; } = null!;

    public RenameColumnOperation(string tableName, string columnName, string operationType) : base(tableName, columnName, operationType)
    {
    }
}
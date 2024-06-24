using OrmLibrary.Enums;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class ChangeNullabilityColumnOperation : AlterColumnMigrationOperation
{
    public SqlType ColumnType { get; set; }
    public bool IsNullable { get; set; }
    
    public ChangeNullabilityColumnOperation(string tableName, string columnName, ColumnOperationType operationType) : base(tableName, columnName, operationType)
    {
    }
}
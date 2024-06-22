using OrmLibrary.Enums;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class ChangeNullabilityColumnOperation : AlterColumnMigrationOperation
{
    public SqlType ColumnType { get; set; }
    public bool IsNullable { get; set; }
    
    public ChangeNullabilityColumnOperation(string tableName, string columnName, string operationType) : base(tableName, columnName, operationType)
    {
    }
}
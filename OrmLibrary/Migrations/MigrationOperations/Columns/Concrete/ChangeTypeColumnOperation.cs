using OrmLibrary.Enums;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class ChangeTypeColumnOperation : AlterColumnMigrationOperation
{
    public SqlType NewType { get; set; }

    public ChangeTypeColumnOperation() { }
    
    public ChangeTypeColumnOperation(string tableName, string columnName, ColumnOperationType operationType) : base(tableName, columnName, operationType)
    {
    }
}
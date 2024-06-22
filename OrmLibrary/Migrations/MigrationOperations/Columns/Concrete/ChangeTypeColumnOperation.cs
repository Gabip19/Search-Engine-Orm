using OrmLibrary.Enums;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class ChangeTypeColumnOperation : AlterColumnMigrationOperation
{
    public SqlType NewType { get; set; }
    
    public ChangeTypeColumnOperation(string tableName, string columnName, string operationType) : base(tableName, columnName, operationType)
    {
    }
}
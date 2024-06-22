using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public abstract class AlterColumnMigrationOperation : IAlterColumnMigrationOperation
{
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public string OperationType { get; set; }

    protected AlterColumnMigrationOperation(string tableName, string columnName, string operationType)
    {
        TableName = tableName;
        ColumnName = columnName;
        OperationType = operationType;
    }
}
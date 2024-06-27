namespace OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

public abstract class AlterColumnMigrationOperation : IAlterColumnMigrationOperation
{
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public ColumnOperationType OperationType { get; set; }

    protected AlterColumnMigrationOperation() { }
    
    protected AlterColumnMigrationOperation(string tableName, string columnName, ColumnOperationType operationType)
    {
        TableName = tableName;
        ColumnName = columnName;
        OperationType = operationType;
    }
}
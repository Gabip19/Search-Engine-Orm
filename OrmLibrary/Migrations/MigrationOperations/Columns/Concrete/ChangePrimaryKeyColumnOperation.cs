﻿namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class ChangePrimaryKeyColumnOperation : AlterColumnMigrationOperation
{
    public bool IsPrimaryKey { get; set; }
    
    public ChangePrimaryKeyColumnOperation(string tableName, string columnName, ColumnOperationType operationType) : base(tableName, columnName, operationType)
    {
    }
}
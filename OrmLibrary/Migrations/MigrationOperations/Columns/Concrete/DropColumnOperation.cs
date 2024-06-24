﻿using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class DropColumnOperation : IDropColumnMigrationOperation
{
    public string TableName { get; set; }
    public ColumnOperationType OperationType { get; set; }
    public string ColumnName { get; set; }
}
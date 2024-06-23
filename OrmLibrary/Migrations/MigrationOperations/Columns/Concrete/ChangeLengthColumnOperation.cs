﻿using OrmLibrary.Enums;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class ChangeLengthColumnOperation : AlterColumnMigrationOperation
{
    public SqlType ColumnType { get; set; }
    public int? Length { get; set; }
    
    public ChangeLengthColumnOperation(string tableName, string columnName, string operationType) : base(tableName, columnName, operationType)
    {
    }
}
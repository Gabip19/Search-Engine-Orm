﻿using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

public class DropConstraintOperation : IDropConstraintMigrationOperation
{
    public string TableName { get; set; }
    public string OperationType { get; set; }
    public string ConstraintName { get; set; }
}
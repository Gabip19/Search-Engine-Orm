using OrmLibrary.Enums;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using OrmLibrary.Serialization;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

public class AddForeignKeyConstraintOperation : AddConstraintOperation
{
    public ForeignKeyGroupDto ForeignKeyGroupDto { get; set; }

    public AddForeignKeyConstraintOperation(string tableName, TableOperationType operationType, string constraintName, TableConstraintType constraintType) : base(tableName, operationType, constraintName, constraintType)
    {
    }
}
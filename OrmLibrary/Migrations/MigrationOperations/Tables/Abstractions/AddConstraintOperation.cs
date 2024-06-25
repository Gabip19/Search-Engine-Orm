using OrmLibrary.Enums;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public abstract class AddConstraintOperation : IAddConstraintMigrationOperation
{
    public string TableName { get; set; }
    public TableOperationType OperationType { get; set; }
    public string ConstraintName { get; set; }
    public TableConstraintType ConstraintType { get; set; }

    protected AddConstraintOperation(string tableName, TableOperationType operationType, string constraintName, TableConstraintType constraintType)
    {
        TableName = tableName;
        OperationType = operationType;
        ConstraintName = constraintName;
        ConstraintType = constraintType;
    }
}
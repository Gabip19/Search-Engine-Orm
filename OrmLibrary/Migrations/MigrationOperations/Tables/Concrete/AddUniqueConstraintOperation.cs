using OrmLibrary.Enums;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

public class AddUniqueConstraintOperation : AddConstraintOperation 
{
    public string ColumnName { get; set; }
    
    public AddUniqueConstraintOperation(string tableName, TableOperationType operationType, string constraintName, TableConstraintType constraintType) : base(tableName, operationType, constraintName, constraintType)
    {
    }
}
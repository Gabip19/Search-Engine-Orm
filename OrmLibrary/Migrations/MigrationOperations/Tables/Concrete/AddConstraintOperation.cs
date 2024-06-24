using OrmLibrary.Constraints;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

public class AddConstraintOperation : IAddConstraintMigrationOperation
{
    public string TableName { get; set; }
    public TableOperationType OperationType { get; set; }
    public string ConstraintName { get; set; }
    public ITableConstraint NewConstraint { get; set; }
}
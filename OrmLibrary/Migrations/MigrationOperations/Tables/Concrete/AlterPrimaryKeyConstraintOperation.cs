using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

public class AlterPrimaryKeyConstraintOperation : IAlterConstraintMigrationOperation
{
    public string TableName { get; set; }
    public TableOperationType OperationType { get; set; }
    public string ConstraintName { get; set; }
    public List<string> PrimaryKeyColumns { get; set; }
}
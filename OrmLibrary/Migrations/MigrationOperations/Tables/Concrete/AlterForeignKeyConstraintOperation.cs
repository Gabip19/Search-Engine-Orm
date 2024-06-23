using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using OrmLibrary.Serialization;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

public class AlterForeignKeyConstraintOperation : IAlterConstraintMigrationOperation
{
    public string TableName { get; set; }
    public string OperationType { get; set; }
    public string ConstraintName { get; set; }

    public ForeignKeyGroupDto KeyGroupDto { get; set; }
}
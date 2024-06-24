namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public interface IConstraintMigrationOperation : IAlterTableMigrationOperation
{
    public string ConstraintName { get; set; }
}
namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public interface IConstraintMigrationOperation : ITableMigrationOperation
{
    public string ConstraintName { get; set; }
}
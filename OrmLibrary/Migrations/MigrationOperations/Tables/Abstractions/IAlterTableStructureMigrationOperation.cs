namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public interface IAlterTableStructureMigrationOperation : IAlterTableMigrationOperation
{
    public ColumnsOperationsCollection ColumnOperations { get; set; }
}
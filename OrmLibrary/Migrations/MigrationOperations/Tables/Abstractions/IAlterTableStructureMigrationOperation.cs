namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public interface IAlterTableStructureMigrationOperation : ITableMigrationOperation
{
    public ColumnsOperationsCollection ColumnOperations { get; set; }
}
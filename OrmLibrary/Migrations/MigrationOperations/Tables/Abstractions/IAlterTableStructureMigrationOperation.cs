using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public interface IAlterTableStructureMigrationOperation : ITableMigrationOperation
{
    public IList<IColumnMigrationOperation> ColumnOperations { get; set; }
}
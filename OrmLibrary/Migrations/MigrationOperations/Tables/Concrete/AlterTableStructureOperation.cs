using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

public class AlterTableStructureOperation : IAlterTableStructureMigrationOperation
{
    public string TableName { get; set; }
    public string OperationType { get; set; }
    public IList<IColumnMigrationOperation> ColumnOperations { get; set; }
}
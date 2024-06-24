using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

public class AlterTableStructureOperation : IAlterTableStructureMigrationOperation
{
    public string TableName { get; set; }
    public TableOperationType OperationType { get; set; }
    public ColumnsOperationsCollection ColumnOperations { get; set; }
}
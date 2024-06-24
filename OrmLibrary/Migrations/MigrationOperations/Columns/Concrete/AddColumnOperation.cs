using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class AddColumnOperation : IAddColumnMigrationOperation
{
    public string TableName { get; set; }
    public ColumnOperationType OperationType { get; set; }
    public ColumnProperties NewColumnProps { get; set; }
}
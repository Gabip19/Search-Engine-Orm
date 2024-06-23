using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;

public class AddColumnOperation : IAddColumnMigrationOperation
{
    public string TableName { get; set; }
    public string OperationType { get; set; }
    public ColumnProperties NewColumnProps { get; set; }
}
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

public class AddTableOperation : IAddTableMigrationOperation
{
    public string TableName { get; set; }
    public TableOperationType OperationType { get; set; }
    public IList<ColumnProperties> Columns { get; set; }
}
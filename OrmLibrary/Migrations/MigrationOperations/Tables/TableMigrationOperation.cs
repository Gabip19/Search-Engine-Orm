using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations.MigrationOperations.Tables;

public abstract class TableMigrationOperation : IMigrationOperation
{
    public string TableName { get; set; }
}

using OrmLibrary.Migrations.MigrationOperations;
using OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

namespace OrmLibrary.Migrations;

public class ColumnDifference
{
    public List<IColumnMigrationOperation> Operations { get; set; } = new();
}

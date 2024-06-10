using OrmLibrary.Migrations.MigrationOperations;

namespace OrmLibrary.Migrations;

public class ColumnDifference
{
    public List<ColumnMigrationOperation> Operations { get; set; } = new();
}

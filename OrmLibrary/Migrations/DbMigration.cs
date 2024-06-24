using OrmLibrary.Migrations.MigrationOperations;

namespace OrmLibrary.Migrations;

public class DbMigration
{
    public string MigrationId { get; set; }
    public DateTime MigrationDate { get; set; }
    public int DbVersion { get; set; }
    public MigrationOperationsCollection Operations { get; set; }
}
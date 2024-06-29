namespace OrmLibrary.Migrations;

public class MigrationInfo
{
    public string LastAppliedMigration { get; set; }
    public DateTime MigrationDate { get; set; }
    public int DbVersion { get; set; }

    public MigrationInfo() { }
    
    public MigrationInfo(string lastAppliedMigration, DateTime migrationDate, int dbVersion)
    {
        LastAppliedMigration = lastAppliedMigration;
        MigrationDate = migrationDate;
        DbVersion = dbVersion;
    }
}
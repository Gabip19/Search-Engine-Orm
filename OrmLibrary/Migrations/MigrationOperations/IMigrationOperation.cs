namespace OrmLibrary.Migrations.MigrationOperations;

public interface IMigrationOperation
{
    public string TableName { get; set; }
}
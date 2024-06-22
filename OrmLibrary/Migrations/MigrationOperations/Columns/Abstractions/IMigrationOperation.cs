namespace OrmLibrary.Migrations.MigrationOperations.Columns.Abstractions;

public interface IMigrationOperation
{
    public string TableName { get; set; }
}
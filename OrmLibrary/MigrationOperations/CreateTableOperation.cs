namespace OrmLibrary.MigrationOperations;

public class CreateTableOperation : MigrationOperation
{
    public override MigrationOperationType OperationType => MigrationOperationType.CreateTable;
    public TableProperties TableProperties { get; set; }
}
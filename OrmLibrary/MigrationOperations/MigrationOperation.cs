namespace OrmLibrary.MigrationOperations;

public abstract class MigrationOperation
{
    public abstract MigrationOperationType OperationType { get; }
}
namespace OrmLibrary.MigrationOperations;

public enum MigrationOperationType
{
    CreateTable,
    DropTable,
    AddColumn,
    DropColumn,
    AlterColumn
}
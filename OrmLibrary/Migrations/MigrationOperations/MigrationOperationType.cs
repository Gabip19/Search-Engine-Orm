namespace OrmLibrary.Migrations.MigrationOperations;

public enum MigrationOperationType
{
    CreateTable,
    DropTable,
    AddColumn,
    DropColumn,
    AlterColumn
}
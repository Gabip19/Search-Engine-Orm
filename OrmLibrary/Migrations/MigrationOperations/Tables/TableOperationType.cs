namespace OrmLibrary.Migrations.MigrationOperations.Tables;

public enum TableOperationType
{
    DropTable,
    AddTable,
    AlterForeignKey,
    AlterPrimaryKey,
    AlterTable,
    AddConstraint,
    DropConstraint
}
using OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

namespace OrmLibrary.Converters;

public interface ISqlDdlGenerator
{
    string GenerateSql(IAddTableMigrationOperation operation);
    string GenerateSql(IDropTableMigrationOperation operation);
    string GenerateSql(IAlterTableStructureMigrationOperation operation);
    string GenerateSql(AddColumnOperation operation);
    string GenerateSql(DropColumnOperation operation);
    string GenerateSql(ChangeNullabilityColumnOperation operation);
    string GenerateSql(RenameColumnOperation operation);
    string GenerateSql(ChangeTypeColumnOperation operation);
    string GenerateSql(ChangeLengthColumnOperation operation);
    string GenerateSql(ChangePrimaryKeyColumnOperation operation);
    string GenerateSql(AddForeignKeyConstraintOperation operation);
    string GenerateSql(AddUniqueConstraintOperation operation);
    string GenerateSql(AlterForeignKeyConstraintOperation operation);
    string GenerateSql(AlterPrimaryKeyConstraintOperation operation);
    string GenerateSql(IDropConstraintMigrationOperation operation);
}
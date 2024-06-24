namespace OrmLibrary.Migrations.MigrationOperations.Columns;

public enum ColumnOperationType
{
    AddColumn,
    DropColumn,
    ChangeMaxLength,
    ChangeNullability,
    ChangePrimaryKey,
    ChangeDataType,
    RenameColumn
}
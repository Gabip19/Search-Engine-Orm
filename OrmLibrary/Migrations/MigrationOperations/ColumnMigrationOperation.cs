using OrmLibrary.Mappings;

namespace OrmLibrary.Migrations.MigrationOperations;

public class ColumnMigrationOperation
{
    public string OperationType { get; set; }
    public string TableName { get; set; }
    public ColumnProperties Column { get; set; }
    public ColumnProperties? NewColumnProperties { get; set; } // For modify operations

    public ColumnMigrationOperation(string operationType, string tableName, ColumnProperties column, ColumnProperties? newColumnProperties = null)
    {
        OperationType = operationType;
        TableName = tableName;
        Column = column;
        NewColumnProperties = newColumnProperties;
    }
}

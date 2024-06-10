using OrmLibrary.Mappings;

namespace OrmLibrary.Migrations.MigrationOperations;

public class TableMigrationOperation
{
    public string OperationType { get; set; }
    public string TableName { get; set; }
    public string? NewTableName { get; set; }
    public TableProperties? NewTableMapping { get; set; }
    public List<ColumnMigrationOperation> ColumnOperations { get; set; }

    public TableMigrationOperation(string operationType, string tableName, string? newTableName = null, TableProperties? newTableMapping = null)
    {
        OperationType = operationType;
        TableName = tableName;
        NewTableName = newTableName;
        NewTableMapping = newTableMapping;
        ColumnOperations = new List<ColumnMigrationOperation>();
    }
}

namespace OrmLibrary.Constraints;

public class UniqueConstraint : ITableConstraint
{
    public string Name { get; set; }
    public string TableName { get; set; }

    public string ColumnName { get; set; }
}
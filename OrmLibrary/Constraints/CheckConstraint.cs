namespace OrmLibrary.Constraints;

public class CheckConstraint : ITableConstraint
{
    public string Name { get; set; }
    public string TableName { get; set; }

    public string ColumnName { get; set; }
    public string Condition { get; set; }
}
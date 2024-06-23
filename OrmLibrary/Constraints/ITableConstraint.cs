namespace OrmLibrary.Constraints;

public interface ITableConstraint
{
    public string Name { get; set; }
    public string TableName { get; set; }
}
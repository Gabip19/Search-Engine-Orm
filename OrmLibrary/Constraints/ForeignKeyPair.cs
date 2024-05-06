namespace OrmLibrary.Constraints;

public class ForeignKeyPair
{
    public string ColumnName { get; set; }
    public string ReferencedColumnName { get; set; }
}
namespace OrmLibrary.Constraints;

public class ForeignKeyConstraint : ITableConstraint
{
    public string Name { get; set; } = null!;

    public TableProperties Table { get; set; }
    public TableProperties ReferencedTable { get; set; }

    public ForeignKeyGroup ForeignKeyGroup { get; set; }
}
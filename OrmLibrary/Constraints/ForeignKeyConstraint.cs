using OrmLibrary.Mappings;

namespace OrmLibrary.Constraints;

public class ForeignKeyConstraint : ITableConstraint
{
    public string Name { get; set; } = null!;

    public string TableName { get; set; }
    public string ReferencedTableName { get; set; }

    public ForeignKeyGroup ForeignKeyGroup { get; set; }
}
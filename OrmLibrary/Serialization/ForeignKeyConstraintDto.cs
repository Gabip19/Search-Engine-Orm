using OrmLibrary.Constraints;

namespace OrmLibrary.Serialization;

public class ForeignKeyConstraintDto : ITableConstraint
{
    public string Name { get; set; } = null!;

    public string TableName { get; set; }
    public string ReferencedTableName { get; set; }

    public ForeignKeyGroupDto ForeignKeyGroup { get; set; }
}
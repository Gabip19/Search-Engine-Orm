using OrmLibrary.Constraints;

namespace OrmLibrary.Serialization;

public class UniqueConstraintDto : ITableConstraint
{
    public string Name { get; set; }
    public string ColumnName { get; set; }
}
using OrmLibrary.Constraints;

namespace OrmLibrary.Serialization;

public class ForeignKeyGroupDto : IForeignKeyGroup
{
    public string AssociatedTableName { get; set; }
    public string AssociatedPropertyName { get; set; }
    public string ReferencedTableName { get; set; }
    public IList<string> Columns { get; set; }
    public IList<string> ReferencedColumns { get; set; }
}
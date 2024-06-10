using OrmLibrary.Mappings;

namespace OrmLibrary.Constraints;

public class ForeignKeyPair
{
    public ColumnProperties MainColumn { get; set; }
    public ColumnProperties ReferencedColumn { get; set; }
}
namespace OrmLibrary.Attributes.Relational;

public class ForeignKeyAttribute : AbstractColumnAttribute
{
    public Type ReferencedType { get; set; }
}
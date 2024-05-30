namespace OrmLibrary.Attributes.Relational;

public class OneToOneAttribute : AbstractColumnAttribute
{
    public Type MappedBy { get; set; }
}
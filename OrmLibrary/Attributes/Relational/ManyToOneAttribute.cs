namespace OrmLibrary.Attributes.Relational;

public class ManyToOneAttribute : AbstractColumnAttribute
{
    public string? ReversedProperty { get; set; }
}
namespace OrmLibrary.Attributes.Relational;

public class OneToManyAttribute : AbstractColumnAttribute
{
    public string? ReversedProperty { get; set; }
}
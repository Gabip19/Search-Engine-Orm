namespace OrmLibrary.Attributes.Relational;

public class OneToManyAttribute : AbstractRelationalAttribute
{
    public string ReversedPropertyName { get; set; }

    public OneToManyAttribute(string reversedPropertyName)
    {
        ReversedPropertyName = reversedPropertyName;
    }
}
namespace OrmLibrary.Attributes.Relational;

public class OneToManyAttribute : AbstractColumnAttribute
{
    public string ReversedPropertyName { get; set; }

    public OneToManyAttribute(string reversedPropertyName)
    {
        ReversedPropertyName = reversedPropertyName;
    }
}
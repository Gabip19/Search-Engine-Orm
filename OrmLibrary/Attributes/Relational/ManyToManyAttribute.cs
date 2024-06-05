namespace OrmLibrary.Attributes.Relational;

public class ManyToManyAttribute : AbstractColumnAttribute
{
    public Type? MappingType { get; set; }
    public string ReversedPropertyName { get; set; }

    public ManyToManyAttribute(string reversedPropertyName, Type? mappingType = null)
    {
        MappingType = mappingType;
        ReversedPropertyName = reversedPropertyName;
    }
}
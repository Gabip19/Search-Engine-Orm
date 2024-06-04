namespace OrmLibrary.Attributes.Relational;

public class OneToOneAttribute : AbstractColumnAttribute
{
    public string? MappedByColumnName { get; set; }

    public OneToOneAttribute() { }
    
    public OneToOneAttribute(string mappedByColumnName)
    {
        MappedByColumnName = mappedByColumnName;
    }
}
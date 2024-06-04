namespace OrmLibrary.Attributes.Relational;

public class ForeignKeyAttribute : AbstractColumnAttribute
{
    public Type ReferencedType { get; set; }
    public string ReferencedColumnName { get; set; }

    public ForeignKeyAttribute(Type referencedType, string referencedColumnName)
    {
        ReferencedType = referencedType;
        ReferencedColumnName = referencedColumnName;
    }
}
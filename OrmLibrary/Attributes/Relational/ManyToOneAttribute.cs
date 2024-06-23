namespace OrmLibrary.Attributes.Relational;

public class ManyToOneAttribute : AbstractRelationalAttribute
{
    public string? ColumnsNamesPrefix { get; set; }
    
    public ManyToOneAttribute() { }
    
    public ManyToOneAttribute(string columnsNamesPrefix) => ColumnsNamesPrefix = columnsNamesPrefix;
}
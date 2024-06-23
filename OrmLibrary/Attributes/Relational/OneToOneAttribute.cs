namespace OrmLibrary.Attributes.Relational;

public class OneToOneAttribute : AbstractRelationalAttribute
{
    public string? MappedByColumnName { get; set; }
    public string? ColumnsNamesPrefix { get; set; }
    
    public OneToOneAttribute() { }
    
    public OneToOneAttribute(string? mappedByColumnName = null, string? columnsNamesPrefix = null)
    {
        MappedByColumnName = mappedByColumnName;
        ColumnsNamesPrefix = columnsNamesPrefix;
    }
}
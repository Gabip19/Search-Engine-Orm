namespace OrmLibrary.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : AbstractColumnAttribute
{
    public string? Name { get; set; }
    public ColumnAttribute(string name) => Name = name;
    public ColumnAttribute() {}
}
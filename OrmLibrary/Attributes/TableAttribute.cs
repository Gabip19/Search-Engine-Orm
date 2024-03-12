namespace OrmLibrary.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute : Attribute
{
    public string Name { get; private set; }

    public TableAttribute(string name) => Name = name;
}
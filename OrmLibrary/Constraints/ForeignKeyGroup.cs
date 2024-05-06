using System.Reflection;

namespace OrmLibrary.Constraints;

public class ForeignKeyGroup
{
    public PropertyInfo AssociatedProperty { get; set; }
    public ICollection<ForeignKeyPair> KeyPairs { get; set; } = new List<ForeignKeyPair>();
}
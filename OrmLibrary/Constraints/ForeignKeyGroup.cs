using System.Reflection;

namespace OrmLibrary.Constraints;

public class ForeignKeyGroup : IForeignKeyGroup
{
    public PropertyInfo AssociatedProperty { get; set; }
    public IList<ForeignKeyPair> KeyPairs { get; set; } = new List<ForeignKeyPair>();
}
using System.Reflection;

namespace OrmLibrary;

internal static class OrmContext
{
    public static Assembly[] DomainAssemblies { get; set; }
    public static Assembly PersistanceAssembly { get; set; }
    public static ICollection<Type> MappedTypes { get; set; }
}
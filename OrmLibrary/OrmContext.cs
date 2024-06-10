using System.Reflection;
using OrmLibrary.Mappings;

namespace OrmLibrary;

internal static class OrmContext
{
    public static string ConnectionString { get; set; }
    public static Assembly[] DomainAssemblies { get; set; }
    public static Assembly PersistanceAssembly { get; set; }
    public static ICollection<Type> MappedTypes { get; set; }
    public static CurrentEntityModels CurrentEntityModels { get; internal set; }
}
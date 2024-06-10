using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OrmLibrary.Mappings;

namespace OrmLibrary.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection ConfigureOrmStartup(this IServiceCollection services, Assembly persistenceAssembly,
        Assembly[] domainAssemblies)
    {
        // Console.WriteLine("\n\n");
        //
        // Console.WriteLine($"Persistence assembly: {persistenceAssembly.FullName}");
        // Console.WriteLine($"Persistence assembly: {persistenceAssembly.GetName().FullName}");
        // Console.WriteLine($"Persistence assembly: {persistenceAssembly.GetName().Name}");
        // Console.WriteLine();
        //
        // var assemblyLocation = persistenceAssembly.Location;
        // Console.WriteLine(mappingEntities[0].FullyQualifiedName);
            // .GetTypes().Where(type => type.GetCustomAttribute<TableAttribute>() != null);
        
        // var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.IsDynamic);
        
        // File.GetLastAccessTime();
        // Console.WriteLine("1: " + AppDomain.CurrentDomain.BaseDirectory);
        // Console.WriteLine("2: " + Directory.GetCurrentDirectory());
        
        var schemasDirectoryPath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}DbSchemas";
        // Console.WriteLine(schemasDirectoryPath);
        
        // Directory.CreateDirectory(schemasDirectoryPath);
        
        // Directory.GetFileSystemEntries("");
        
        // OrmContext.PersistanceAssembly = persistenceAssembly;
        OrmContext.DomainAssemblies = domainAssemblies;
        // var a =  domainAssemblies[0].GetTypes();
        // OrmContext.MappedTypes = domainAssemblies
        //     .SelectMany(
        //     assembly => assembly.GetTypes().Where(type => type.GetCustomAttribute<TableAttribute>() != null),
        //     (_, type) => type
        // ).ToList();
        
        var currentEntityModels = CurrentSchemaLoader.LoadCurrentSchema(schemasDirectoryPath);
        
        // MigrationManager.CheckForChanges(currentEntityModels);
        
        Console.WriteLine("\n\nDone");
        return services;
    }
}
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations;
using OrmLibrary.Serialization;

namespace OrmLibrary.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection ConfigureOrmStartup(this IServiceCollection services, string connectionString, Assembly persistenceAssembly,
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
        OrmContext.MappedTypes = domainAssemblies
            .SelectMany(
            assembly => assembly.GetTypes().Where(type => type.IsMappedEntityType()),
            (_, type) => type
        ).ToList();

        var currentEntityModels = CurrentSchemaLoader.LoadCurrentSchema(schemasDirectoryPath);
        OrmContext.CurrentEntityModels = currentEntityModels;
        OrmContext.ConnectionString = connectionString;
        
        // var readJson = File.ReadAllText(Path.Combine(schemasDirectoryPath, "test_schema.json"));
        // var desCurrentEntityModels = serializer.DeserializeCurrentEntityModels(readJson);
        
        var migrationOperations = MigrationManager.CheckForChanges(currentEntityModels);

        // if (currentEntityModels.HasChanged)
        // {
        //     Console.WriteLine("Found changes... Writing to file...");
        //     var serializer = new SchemaSerializer();
        //
        //     var json = serializer.SerializeCurrentEntityModels(currentEntityModels);
        //     File.WriteAllText(Path.Combine(schemasDirectoryPath, "current_db_schema.json"), json);
        // }
        
        Console.WriteLine("\n\nDone");
        return services;
    }
}
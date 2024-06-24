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
        // var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.IsDynamic);
        
        var schemasDirectoryPath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}DbSchemas";
        // Console.WriteLine(schemasDirectoryPath);
        
        // Directory.CreateDirectory(schemasDirectoryPath);
        
        OrmContext.PersistanceAssembly = persistenceAssembly;
        OrmContext.DomainAssemblies = domainAssemblies;
        OrmContext.MappedTypes = domainAssemblies
            .SelectMany(
            assembly => assembly.GetTypes().Where(type => type.IsMappedEntityType()),
            (_, type) => type
        ).ToList();
        OrmContext.ConnectionString = connectionString;
        
        var currentEntityModels = CurrentSchemaLoader.LoadCurrentSchema(schemasDirectoryPath);
        // OrmContext.CurrentEntityModels = currentEntityModels;
        
        var migrationOperations = MigrationManager.GetMigrationOperations(currentEntityModels);

        MigrationManager.GenerateMigrationFile(migrationOperations,
            $"{schemasDirectoryPath}{Path.DirectorySeparatorChar}Migrations");
        
        if (OrmContext.CurrentEntityModels.HasChanged)
        {
            Console.WriteLine("Found changes... Writing to file...");
            var serializer = new SchemaSerializer();
        
            var json = serializer.SerializeCurrentEntityModels(OrmContext.CurrentEntityModels);
            File.WriteAllText(Path.Combine(schemasDirectoryPath, "current_db_schema.json"), json);
        }
        
        Console.WriteLine("\n\nDone");
        return services;
    }
}
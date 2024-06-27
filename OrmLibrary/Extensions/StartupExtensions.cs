using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations;
using OrmLibrary.Serialization;

namespace OrmLibrary.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection ConfigureOrmStartup(this IServiceCollection services, string connectionString,
        Assembly persistenceAssembly, Assembly[] domainAssemblies)
    {
        // var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.IsDynamic);
        
        var schemasDirectoryPath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}DbSchemas";
        
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

        string sql;
        
        if (migrationOperations.Any())
        {
            Console.WriteLine("Found migration operations. Generating migration file...");
            
            MigrationManager.GenerateMigrationFile(migrationOperations, Path.Combine(schemasDirectoryPath, "Migrations"));
            
            var migrationPath = Path.Combine(schemasDirectoryPath, "Migrations", "TEST_Migration.json");
            sql = MigrationManager.ApplyMigration(migrationPath);
        }
        
        // TODO: majuscule in .json la proprietati
        // TODO: make sure the foreign keys reference a unique column - leave it for db to check?

        if (OrmContext.CurrentEntityModels.HasChanged)
        {
            Console.WriteLine("Found changes... Writing to file...");
            
            var serializer = new SchemaSerializer();
            var json = serializer.SerializeCurrentEntityModels(OrmContext.CurrentEntityModels);
            // File.WriteAllText(Path.Combine(schemasDirectoryPath, "current_db_schema.json"), json);
        }
        
        Console.WriteLine("\n\nDone");
        return services;
    }
}
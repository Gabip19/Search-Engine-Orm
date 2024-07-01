using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        
        var schemasDirectoryPath = Path.Combine(ExtensionsHelper.GetAssemblyPath(persistenceAssembly), "DbSchemas");
        
        OrmContext.PersistanceAssembly = persistenceAssembly;
        OrmContext.DomainAssemblies = domainAssemblies;
        OrmContext.MappedTypes = domainAssemblies
            .SelectMany(
            assembly => assembly.GetTypes().Where(type => type.IsMappedEntityType()),
            (_, type) => type
        ).ToList();
        OrmContext.ConnectionString = connectionString;
        OrmContext.SchemasDirectoryPath = schemasDirectoryPath;
        
        return services;
    }

    public static IApplicationBuilder UseOrmMappings(this IApplicationBuilder app, IHostEnvironment env)
    {
        //// Load current model
        var currentEntityModels = CurrentSchemaLoader.LoadCurrentSchema(OrmContext.SchemasDirectoryPath);
        
        // TODO: datetime
        //// Check for changes and generate migrations
        if (env.IsDevelopment()) // TODO: maybe add support for different user defined environments
        {
            MigrationManager.CheckForSchemaUpdates(currentEntityModels);
        }
        else
        {
            OrmContext.CurrentEntityModels = currentEntityModels ?? 
                throw new ArgumentException("Current entities model is null and can not be generated in the current environment");
        }
        
        MigrationManager.UpdateDatabase();
        
        //// Update current_db_schema if there are any changes
        if (OrmContext.CurrentEntityModels.HasChanged)
        {
            Console.WriteLine("Found changes... Writing to file...");
            
            var serializer = new SchemaSerializer();
            var json = serializer.SerializeCurrentEntityModels(OrmContext.CurrentEntityModels);
            File.WriteAllText(Path.Combine(OrmContext.SchemasDirectoryPath, "current_db_schema.json"), json);
        }
        
        // TODO: majuscule in .json la proprietati
        // TODO: make sure the foreign keys reference a unique column - leave it for db to check?
        
        Console.WriteLine("\n\nDone");
        
        return app;
    }
}
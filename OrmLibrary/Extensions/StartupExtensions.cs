using System.Data.Common;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrmLibrary.Abstractions;
using OrmLibrary.Execution;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations;
using OrmLibrary.Serialization;

namespace OrmLibrary.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection ConfigureOrmStartup(this IServiceCollection services, string connectionString,
        Assembly persistenceAssembly, Assembly[] domainAssemblies)
    {
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

        services.AddSingleton<IDbSchemaExtractor, DbSchemaExtractor>();
        services.AddSingleton<IMigrationManager, MigrationManager>();
        services.AddSingleton<IDbContextFactory, ScopedDbContextFactory>();
        
        return services;
    }

    public static IApplicationBuilder UseOrmMappings(this IApplicationBuilder app, IHostEnvironment env)
    {
        var migrationManager = app.ApplicationServices.GetRequiredService<IMigrationManager>();
        
        //// Load current model
        var currentEntityModels = CurrentSchemaLoader.LoadCurrentSchema(OrmContext.SchemasDirectoryPath);
        
        //// Check for changes and generate migrations
        if (env.IsDevelopment())
        {
            migrationManager.CheckForSchemaUpdates(currentEntityModels);
        }
        else
        {
            OrmContext.CurrentEntityModels = currentEntityModels ?? 
                throw new ArgumentException("Current entities model is null and can not be generated in the current environment");
        }

        try
        {
            migrationManager.UpdateDatabase();
        }
        catch (DbException e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            //// Update current_db_schema if there are any changes
            if (OrmContext.CurrentEntityModels.HasChanged)
            {
                Console.WriteLine("Found changes... Writing to file...");
            
                var serializer = new SchemaSerializer();
                var json = serializer.SerializeCurrentEntityModels(OrmContext.CurrentEntityModels);
                File.WriteAllText(Path.Combine(OrmContext.SchemasDirectoryPath, "current_db_schema.json"), json);
            }
        }
        
        Console.WriteLine("\n\nDone");
        
        return app;
    }
}
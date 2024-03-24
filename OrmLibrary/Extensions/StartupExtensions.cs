using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OrmLibrary.Attributes;

namespace OrmLibrary.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection ConfigureOrmStartup(this IServiceCollection services, Assembly persistenceAssembly)
    {
        Console.WriteLine("\n\n");

        Console.WriteLine(persistenceAssembly.FullName);

        var mappingEntities = persistenceAssembly.GetTypes()
            .Where(type => type.GetCustomAttribute<TableAttribute>() != null);

        foreach (var entity in mappingEntities)
        {
            foreach (var prop in entity.GetProperties())
            { 
                var a = DbSchemaExtractor.ExtractColumnProperties(prop);
            }

            // Console.WriteLine("Class name: " + entity.Name);
            // Console.WriteLine("Table name: " + entity.GetCustomAttribute<TableAttribute>()!.Name);
            // Console.WriteLine("Properties: ");
            // entity.GetProperties().ToList().ForEach(prop =>
            // {
            //     var c = prop.GetCustomAttributes<AbstractColumnAttribute>();
            //
            //     // Console.WriteLine(prop.Name);
            //     // Console.WriteLine(prop.PropertyType);
            // });
            // Console.WriteLine("\n");
        }

        Console.WriteLine("\n\n");
        return services;
    }
}
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

        var tableMappings = DbSchemaExtractor.ExtractTablesProperties(mappingEntities);

        Console.WriteLine("\n\nDone");
        return services;
    }
}
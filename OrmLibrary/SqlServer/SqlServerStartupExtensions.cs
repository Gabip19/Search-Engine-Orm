using Microsoft.Extensions.DependencyInjection;
using OrmLibrary.Converters;
using OrmLibrary.Execution;

namespace OrmLibrary.SqlServer;

public static class SqlServerStartupExtensions
{
    public static IServiceCollection UseSqlServer(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionProvider, SqlServerConnectionProvider>();
        services.AddSingleton<ISqlDdlGenerator, SqlServerDdlGenerator>();
        services.AddSingleton<ISqlQueryGenerator, SqlServerQueryGenerator>();
        services.AddSingleton<ISqlTypeConverter, SqlServerTypeConverter>();
        
        return services;
    }
}
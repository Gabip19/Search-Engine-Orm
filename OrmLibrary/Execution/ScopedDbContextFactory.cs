using Microsoft.Extensions.DependencyInjection;
using OrmLibrary.Abstractions;

namespace OrmLibrary.Execution;

public class ScopedDbContextFactory : IDbContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ScopedDbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ScopedDbContext CreateContext()
    {
        var sqlGenerator = _serviceProvider.GetRequiredService<ISqlQueryGenerator>();
        var connectionProvider = _serviceProvider.GetRequiredService<IConnectionProvider>();
        return new ScopedDbContext(connectionProvider, sqlGenerator);
    }
}

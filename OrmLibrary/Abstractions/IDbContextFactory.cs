using OrmLibrary.Execution;

namespace OrmLibrary.Abstractions;

public interface IDbContextFactory
{
    ScopedDbContext CreateContext();
}
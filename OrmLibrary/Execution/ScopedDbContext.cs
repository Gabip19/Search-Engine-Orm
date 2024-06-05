namespace OrmLibrary;

public class ScopedDbContext : IDisposable
{
    DbTable<TEntity> Entity<TEntity>() where TEntity : class, new()
    {
        return new DbTable<TEntity>();
    }
    
    public void Dispose()
    {
        // TODO release managed resources here
    }
}
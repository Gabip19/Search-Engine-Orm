namespace OrmLibrary;

public class DbTable<TEntity> where TEntity : class, new()
{
    public QueryBuilder<TEntity> Query()
    {
        return new QueryBuilder<TEntity>();
    }
}
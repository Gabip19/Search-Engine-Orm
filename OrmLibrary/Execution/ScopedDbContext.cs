using System.Data;
using OrmLibrary.SqlServer;

namespace OrmLibrary.Execution;

public class ScopedDbContext : IDisposable
{
    private readonly ISqlQueryGenerator _sqlGenerator;
    
    private readonly IConnectionProvider _connectionProvider;
    
    private readonly DbQueryExecutor _dbQueryExecutor = new();
    private readonly DbCommandExecutor _dbCommandExecutor = new();
    
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    
    public ScopedDbContext(IConnectionProvider connectionProvider, ISqlQueryGenerator generator)
    {
        _connectionProvider = connectionProvider;
        _sqlGenerator = generator;
        _connection = _connectionProvider.CreateConnection();
        _connection.Open();
    }
    
    public DbTable<TEntity> Entity<TEntity>() where TEntity : class, new()
    {
        return new DbTable<TEntity>(this);
    }
    
    public QueryExecutionResult<TEntity> Execute<TEntity>(QueryContext<TEntity> queryContext) where TEntity : class, new()
    {
        var connection = GetOpenedConnection();
        
        var sqlQuery = _sqlGenerator.GenerateQuery(queryContext);
        
        if (_transaction is null)
        {
            BeginTransaction();
            try
            {
                var result = _dbQueryExecutor.ExecuteQuery<TEntity>(sqlQuery, connection, _transaction!);
                CommitTransaction();
                return result;
            }
            catch (Exception)
            {
                RollbackTransaction();
                throw;
            }
        }

        return _dbQueryExecutor.ExecuteQuery<TEntity>(sqlQuery, connection, _transaction);
    }
    
    public int ExecuteSqlCommand(string sql)
    {
        var connection = GetOpenedConnection();
        
        if (_transaction is null)
        {
            BeginTransaction();
            try
            {
                var result = _dbCommandExecutor.ExecuteCommand(sql, connection, _transaction!);
                CommitTransaction();
                return result;
            }
            catch (Exception)
            {
                RollbackTransaction();
                throw;
            }
        }
        
        return _dbCommandExecutor.ExecuteCommand(sql, connection, _transaction);
    }
    
    private IDbConnection GetOpenedConnection()
    {
        if (_connection == null)
        {
            _connection = _connectionProvider.CreateConnection();
            _connection.Open();
        }
        else if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
        }

        return _connection;
    }
    
    public void BeginTransaction()
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already started on this context.");
        }
        _transaction = GetOpenedConnection().BeginTransaction();
    }
    
    public void CommitTransaction()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is started on this context.");
        }
        _transaction.Commit();
        _transaction.Dispose();
        _transaction = null;
    }
    
    public void RollbackTransaction()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is started on this context.");
        }
        _transaction.Rollback();
        _transaction.Dispose();
        _transaction = null;
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
    }
}
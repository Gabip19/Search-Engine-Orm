using System.Data;
using System.Data.SqlClient;
using OrmLibrary.Execution;

namespace OrmLibrary.SqlServer;

public class SqlServerConnectionProvider : IConnectionProvider
{
    private readonly string _connectionString;

    public SqlServerConnectionProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
using System.Data;

namespace OrmLibrary.Execution;

public interface IConnectionProvider
{
    IDbConnection CreateConnection();
}
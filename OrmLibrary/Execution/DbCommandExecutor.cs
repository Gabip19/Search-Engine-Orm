using System.Data;

namespace OrmLibrary.Execution;

public class DbCommandExecutor
{
    public int ExecuteCommand(string sql, IDbConnection connection, IDbTransaction transaction)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;

        return command.ExecuteNonQuery();
    }
}
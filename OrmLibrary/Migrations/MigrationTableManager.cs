using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using OrmLibrary.Execution;

namespace OrmLibrary.Migrations;

public class MigrationTableManager
{
    private readonly IConnectionProvider _connectionFactory;
    private const string TableName = "__DbMigrations__";

    public MigrationTableManager(IConnectionProvider connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public MigrationInfo GetLastMigrationInfo()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT LastAppliedMigration, MigrationDate, DbVersion FROM {TableName}";

            using var reader = command.ExecuteReader();

            if (!reader.Read()) return InsertDefaultInfo(connection);
            
            var lastAppliedMigration = reader.GetString(0);
            var migrationDate = reader.GetDateTime(1);
            var dbVersion = reader.GetInt32(2);

            return new MigrationInfo(lastAppliedMigration, migrationDate, dbVersion);
        }
        catch (DbException ex)
        {
            if (!IsTableNotExistException(ex)) throw;
            
            CreateMigrationTable(connection);
            return InsertDefaultInfo(connection);
        }
    }

    private MigrationInfo InsertDefaultInfo(IDbConnection connection)
    {
        var defaultInfo = new MigrationInfo("0_None", DateTime.Parse("1/1/1753 12:00:00"), 0);
        InsertMigrationInfo(connection, defaultInfo);

        return defaultInfo;
    }

    private static bool IsTableNotExistException(Exception ex)
    {
        return ex is SqlException { Number: 208 } ||
        ex.Message.Contains("does not exist") || ex.Message.Contains("Invalid object name");
    }

    private void CreateMigrationTable(IDbConnection connection)
    {
        // using var command = connection.CreateCommand();
        // command.CommandText = $@"
        //         CREATE TABLE {TableName} (
        //             LastAppliedMigration VARCHAR(255) NOT NULL PRIMARY KEY,
        //             MigrationDate DATETIME NOT NULL,
        //             DbVersion INT NOT NULL
        //         )";
        //
        // command.ExecuteNonQuery();

        const string Sql = $@"
                CREATE TABLE {TableName} (
                    LastAppliedMigration VARCHAR(255) NOT NULL PRIMARY KEY,
                    MigrationDate DATETIME NOT NULL,
                    DbVersion INT NOT NULL
                )";

        using var dbContext = new ScopedDbContext();
        dbContext.ExecuteSqlCommand(Sql);
    }
    
    private void InsertMigrationInfo(IDbConnection connection, MigrationInfo migrationInfo)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $@"
                INSERT INTO {TableName} (LastAppliedMigration, MigrationDate, DbVersion) 
                VALUES (@LastAppliedMigration, @MigrationDate, @DbVersion)";

        var lastAppliedMigrationParam = command.CreateParameter();
        lastAppliedMigrationParam.ParameterName = "@LastAppliedMigration";
        lastAppliedMigrationParam.Value = migrationInfo.LastAppliedMigration;
        command.Parameters.Add(lastAppliedMigrationParam);

        var migrationDateParam = command.CreateParameter();
        migrationDateParam.ParameterName = "@MigrationDate";
        migrationDateParam.Value = migrationInfo.MigrationDate;
        command.Parameters.Add(migrationDateParam);

        var dbVersionParam = command.CreateParameter();
        dbVersionParam.ParameterName = "@DbVersion";
        dbVersionParam.Value = migrationInfo.DbVersion;
        command.Parameters.Add(dbVersionParam);

        command.ExecuteNonQuery();
    }

    public void UpdateLastMigrationInfo(MigrationInfo migrationInfo)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $@"
                    UPDATE {TableName} 
                    SET LastAppliedMigration = @LastAppliedMigration, 
                        MigrationDate = @MigrationDate, 
                        DbVersion = @DbVersion";

        var lastAppliedMigrationParam = command.CreateParameter();
        lastAppliedMigrationParam.ParameterName = "@LastAppliedMigration";
        lastAppliedMigrationParam.Value = migrationInfo.LastAppliedMigration;
        command.Parameters.Add(lastAppliedMigrationParam);

        var migrationDateParam = command.CreateParameter();
        migrationDateParam.ParameterName = "@MigrationDate";
        migrationDateParam.Value = migrationInfo.MigrationDate;
        command.Parameters.Add(migrationDateParam);

        var dbVersionParam = command.CreateParameter();
        dbVersionParam.ParameterName = "@DbVersion";
        dbVersionParam.Value = migrationInfo.DbVersion;
        command.Parameters.Add(dbVersionParam);

        command.ExecuteNonQuery();
    }
}
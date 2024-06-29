using System.Text;
using OrmLibrary.Converters;
using OrmLibrary.Enums;
using OrmLibrary.Extensions;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations;
using OrmLibrary.Migrations.MigrationOperations.Tables;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;
using OrmLibrary.Serialization;
using OrmLibrary.SqlServer;
using TableOperationsFactory = OrmLibrary.Migrations.MigrationOperations.Tables.TableMigrationOperationsFactory;

namespace OrmLibrary.Migrations;

public static class MigrationManager
{
    private static readonly TableComparer TableComparer = new();
    private static readonly SchemaSerializer SchemaSerializer = new();
    private static readonly ISqlDdlGenerator SqlGenerator = new SqlServerDdlGenerator();
    private static readonly MigrationTableManager MigrationTableManager = new(new SqlServerConnectionProvider(OrmContext.ConnectionString));

    public static void CheckForSchemaUpdates(CurrentEntityModels? currentEntityModels)
    {
        var migrationOperations = GetMigrationOperations(currentEntityModels);
        
        if (migrationOperations.Any())
        {
            Console.WriteLine("Found migration operations. Generating migration file...");
            
            GenerateMigrationFile(migrationOperations, Path.Combine(OrmContext.SchemasDirectoryPath, "Migrations"));
        }
    }
    
    private static MigrationOperationsCollection GetMigrationOperations(CurrentEntityModels? currentEntityModels)
    {
        if (currentEntityModels is not null) 
            return CheckForChanges(currentEntityModels);
        
        OrmContext.CurrentEntityModels = new CurrentEntityModels
        {
            EntitiesMappings = new MappedEntitiesCollection(DbSchemaExtractor.ExtractTablesProperties(OrmContext.MappedTypes)),
            CurrentDbVersion = 1,
            HasChanged = true,
            LastDbUpdate = DateTime.UtcNow
        };

        var migrationOperations = new MigrationOperationsCollection();

        foreach (var tableProps in OrmContext.CurrentEntityModels.EntitiesMappings.Values)
        {
            migrationOperations.AddRange(GetCreateTableOperations(tableProps));
        }
        
        return migrationOperations;
    }
    
    private static MigrationOperationsCollection CheckForChanges(CurrentEntityModels currentEntityModels)
    {
        var operations = new MigrationOperationsCollection();
        
        if (OrmContext.MappedTypes.Any(type =>
                ExtensionsHelper.GetLastModificationDate(type) > currentEntityModels.LastDbUpdate))
        {
            var mappingCollection = new List<TableProperties>();
            var notFoundTypes = OrmContext.MappedTypes.ToHashSet();

            foreach (var lastEntityMapping in currentEntityModels.EntitiesMappings.Values)
            {
                if (lastEntityMapping.AssociatedType is null ||
                    !notFoundTypes.Contains(lastEntityMapping.AssociatedType))
                {
                    operations.Add(TableOperationsFactory.NewDropTableOperation(lastEntityMapping));
                }
                else
                {
                    notFoundTypes.Remove(lastEntityMapping.AssociatedType);

                    var currentEntityMapping =
                        DbSchemaExtractor.ExtractTableProperties(lastEntityMapping.AssociatedType);
                    mappingCollection.Add(currentEntityMapping);

                    operations.AddRange(TableComparer.CompareTables(lastEntityMapping, currentEntityMapping));
                }
            }

            foreach (var newTableProps in notFoundTypes.Select(DbSchemaExtractor.ExtractTableProperties))
            {
                operations.AddRange(GetCreateTableOperations(newTableProps));
                mappingCollection.Add(newTableProps);
            }

            OrmContext.CurrentEntityModels = new CurrentEntityModels
            {
                EntitiesMappings = new MappedEntitiesCollection(mappingCollection),
                CurrentDbVersion = operations.Any()
                    ? ++currentEntityModels.CurrentDbVersion
                    : currentEntityModels.CurrentDbVersion,
                HasChanged = operations.Any(),
            };
        }
        else
        {
            OrmContext.CurrentEntityModels = currentEntityModels;
        }
        
        OrmContext.CurrentEntityModels.LastDbUpdate = DateTime.UtcNow;
        
        return operations;
    }

    private static IList<ITableMigrationOperation> GetCreateTableOperations(TableProperties tableProps)
    {
        var operations = new List<ITableMigrationOperation> { TableOperationsFactory.NewAddTableOperation(tableProps) };

        operations.AddRange(tableProps.Constraints.Select(TableOperationsFactory.NewAddConstraintOperation));

        return operations;
    }

    private static void GenerateMigrationFile(MigrationOperationsCollection migrationOperations, string migrationsFolderPath)
    {
        var migrationDate = OrmContext.CurrentEntityModels.LastDbUpdate;
        var migrationDbVersion = OrmContext.CurrentEntityModels.CurrentDbVersion;
        var migrationId = $"{migrationDate:yyyyMMddThhmmss}_Migration";

        var migration = new DbMigration
        {
            MigrationId = migrationId,
            DbVersion = migrationDbVersion,
            MigrationDate = migrationDate,
            Operations = migrationOperations
        };

        var migrationJson = SchemaSerializer.SerializeDbMigration(migration);

        if (!Directory.Exists(migrationsFolderPath))
        {
            Directory.CreateDirectory(migrationsFolderPath);
        }
        
        // TODO: File.WriteAllText(Path.Combine(migrationsFolderPath, $"{migrationDate:yyyyMMddThhmmss}_Migration.json"), migrationJson);
        File.WriteAllText(Path.Combine(migrationsFolderPath, $"TEST_Migration.json"), migrationJson);
    }

    public static void UpdateDatabase()
    {
        var dbState = MigrationTableManager.GetLastMigrationInfo();
        var currentDbVersion = dbState.DbVersion;
        
        if (dbState.DbVersion >= OrmContext.CurrentEntityModels.CurrentDbVersion) return;
        
        var migrationsFolderPath = Path.Combine(OrmContext.SchemasDirectoryPath, "Migrations");
        string migrationName = null!;
        
        try
        {
            foreach (var migrationFilePath in Directory.EnumerateFiles(migrationsFolderPath).OrderBy(s => s))
            {
                Console.WriteLine(migrationFilePath);
                
                migrationName = Path.GetFileName(migrationFilePath);
                
                if (dbState.LastAppliedMigration.CompareTo(migrationName) < 0)
                {
                    Console.WriteLine($"Applying migration {migrationName}...");
                    ApplyMigration(migrationFilePath);
                    currentDbVersion++;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Failed to migrate database from version {currentDbVersion} to version {currentDbVersion + 1}");
            Console.WriteLine(ex);
            throw;
        }
        finally
        {
            if (dbState.DbVersion != currentDbVersion)
            {
                MigrationTableManager.UpdateLastMigrationInfo(new MigrationInfo
                {
                    DbVersion = currentDbVersion,
                    MigrationDate = DateTime.UtcNow,
                    LastAppliedMigration = migrationName
                });   
            }
        }
    }

    private static void ApplyMigration(string migrationFilePath)
    {
        var json = File.ReadAllText(migrationFilePath);
        var dbMigration = SchemaSerializer.DeserializeDbMigration(json)!;

        var sql = GenerateMigrationSql(dbMigration);
    }

    private static string GenerateMigrationSql(DbMigration dbMigration)
    {
        var startSqlBuilder = new StringBuilder();
        var endSqlBuilder = new StringBuilder();
        var migrationOperations = dbMigration.Operations;

        foreach (var operation in migrationOperations.AlterTableOperations.OfType<AlterForeignKeyConstraintOperation>())
        {
            startSqlBuilder.Append(SqlGenerator.GenerateSql(new DropConstraintOperation
            {
                TableName = operation.TableName,
                ConstraintName = operation.ConstraintName,
                OperationType = TableOperationType.DropConstraint
            }));
            startSqlBuilder.Append("\n\n");
            
            endSqlBuilder.Append(SqlGenerator.GenerateSql(new AddForeignKeyConstraintOperation(
                operation.TableName,
                TableOperationType.AddConstraint,
                operation.ConstraintName,
                TableConstraintType.ForeignKeyConstraint)
                {
                    ForeignKeyGroupDto = operation.KeyGroupDto
                }
            ));
            endSqlBuilder.Append("\n\n");
        }
        
        // drop constraints
        foreach (var operation in migrationOperations.AlterTableOperations.OfType<IDropConstraintMigrationOperation>())
        {
            startSqlBuilder.Append(SqlGenerator.GenerateSql(operation));
            startSqlBuilder.Append("\n\n");
        }
        
        // drop tables
        foreach (var operation in migrationOperations.DropTableOperations)
        {
            startSqlBuilder.Append(SqlGenerator.GenerateSql(operation));
            startSqlBuilder.Append("\n\n");
        }
        
        // add tables
        foreach (var operation in migrationOperations.AddTableOperations)
        {
            startSqlBuilder.Append(SqlGenerator.GenerateSql(operation));
            startSqlBuilder.Append("\n\n");
        }

        // alter primary keys
        foreach (var operation in migrationOperations.AlterTableOperations.OfType<AlterPrimaryKeyConstraintOperation>())
        {
            startSqlBuilder.Append(SqlGenerator.GenerateSql(operation));
            startSqlBuilder.Append("\n\n");
        }
        
        // alter tables
        foreach (var operation in migrationOperations.AlterTableOperations.OfType<IAlterTableStructureMigrationOperation>())
        {
            startSqlBuilder.Append(SqlGenerator.GenerateSql(operation));
            startSqlBuilder.Append("\n\n");
        }

        // alter constraints
        foreach (var operation in migrationOperations.AlterTableOperations.OfType<IAlterConstraintMigrationOperation>())
        {
            if (operation is AlterForeignKeyConstraintOperation || operation is AlterPrimaryKeyConstraintOperation) continue;
            
            startSqlBuilder.Append(SqlGenerator.GenerateSql((dynamic)operation));
            startSqlBuilder.Append("\n\n");
        }
        
        // add constraints
        foreach (var operation in migrationOperations.AlterTableOperations.OfType<IAddConstraintMigrationOperation>())
        {
            startSqlBuilder.Append(SqlGenerator.GenerateSql((dynamic)operation));
            startSqlBuilder.Append("\n\n");
        }

        startSqlBuilder.Append(endSqlBuilder);
        
        return startSqlBuilder.ToString();
    }
}
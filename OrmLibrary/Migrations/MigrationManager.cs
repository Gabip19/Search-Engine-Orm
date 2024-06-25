using OrmLibrary.Extensions;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using OrmLibrary.Serialization;
using TableOperationsFactory = OrmLibrary.Migrations.MigrationOperations.Tables.TableMigrationOperationsFactory;

namespace OrmLibrary.Migrations;

public static class MigrationManager
{
    private static readonly TableComparer TableComparer = new();
    private static readonly SchemaSerializer SchemaSerializer = new();

    public static MigrationOperationsCollection GetMigrationOperations(CurrentEntityModels? currentEntityModels)
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
    
    public static void GenerateMigrationFile(MigrationOperationsCollection migrationOperations, string migrationsFolderPath)
    {
        var migrationDate = OrmContext.CurrentEntityModels.LastDbUpdate;
        var migrationDbVersion = OrmContext.CurrentEntityModels.CurrentDbVersion;
        var migrationId = $"{migrationDate:yyyyMMddThhmmss}_{migrationDbVersion}_Migration";

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
        
        File.WriteAllText(Path.Combine(migrationsFolderPath, $"{migrationDate:yyyyMMddThhmmss}_Migration.json"), migrationJson);
    }
}
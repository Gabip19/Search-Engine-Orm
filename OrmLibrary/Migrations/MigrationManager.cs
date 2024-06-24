using OrmLibrary.Extensions;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using TableOperationsFactory = OrmLibrary.Migrations.MigrationOperations.Tables.TableMigrationOperationsFactory;

namespace OrmLibrary.Migrations;

public static class MigrationManager
{
    private static readonly TableComparer TableComparer = new();

    public static IList<ITableMigrationOperation> GetMigrationOperations(CurrentEntityModels? currentEntityModels)
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

        return OrmContext.CurrentEntityModels.EntitiesMappings.Values
            .Select(TableOperationsFactory.NewAddTableOperation)
            .Cast<ITableMigrationOperation>().ToList();
    }
    
    public static IList<ITableMigrationOperation> CheckForChanges(CurrentEntityModels currentEntityModels)
    {
        var operations = new List<ITableMigrationOperation>();
        
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

            foreach (var mappedType in notFoundTypes.Select(DbSchemaExtractor.ExtractTableProperties))
            {
                operations.Add(TableOperationsFactory.NewAddTableOperation(mappedType));
                mappingCollection.Add(mappedType);
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
    
    // TODO: delete this
    // public static List<TableMigrationOperation> CheckForChanges(CurrentEntityModels currentEntityModels)
    // {
    //     // count mai mic la curent => s-a sters o tabela => gaseste care s-a sters dupa TableName && AssociatedType
    //     // count mai mare la curent => s-a adaugat o tabela => gaseste care s-a adaugat dupa TableName && AssociatedType
    //     
    //     // itereaza current mapped entities
    //         // daca e in last state mapped entities dupa TableName
    //             // check la last write date
    //                 // daca e <= decat last update date
    //                     // continue
    //                 // daca e >= decat last update date
    //                     // fa props check
    //         // daca nu e
    //             // e rename la table?
    //             // e table nou?
    //     // daca nu e in mapped entities dupa
    //
    //     var operations = new List<TableMigrationOperation>();
    //     
    //     var notFoundTypes = OrmContext.MappedTypes.ToHashSet();
    //     var notFoundMappings = new HashSet<TableProperties>();
    //     
    //     foreach (var lastEntityMapping in currentEntityModels.EntitiesMappings.Values)
    //     {
    //         if (OrmContext.MappedTypes.Contains(lastEntityMapping.AssociatedType))
    //         {
    //             notFoundTypes.Remove(lastEntityMapping.AssociatedType);
    //             
    //             var currentEntityMapping = DbSchemaExtractor.ExtractTableProperties(lastEntityMapping.AssociatedType);
    //             
    //             TableComparer.CompareTables(lastEntityMapping, currentEntityMapping);
    //         }
    //         else
    //         {
    //             notFoundMappings.Add(lastEntityMapping);
    //         }
    //     }
    //     
    //     var entityNames = notFoundTypes.ToDictionary(type => type.GetCustomAttribute<TableAttribute>()!.Name);
    //     
    //     foreach (var lastEntityMapping in notFoundMappings)
    //     {
    //         if (entityNames.TryGetValue(lastEntityMapping.Name, out var value))
    //         {
    //             notFoundMappings.Remove(lastEntityMapping);
    //             notFoundTypes.Remove(value);
    //             
    //             var currentEntityMapping = DbSchemaExtractor.ExtractTableProperties(lastEntityMapping.AssociatedType);
    //             operations.AddRange(TableComparer.CompareTables(lastEntityMapping, currentEntityMapping));
    //         }
    //     }
    //
    //     foreach (var notFoundMapping in notFoundMappings)
    //     {
    //         operations.Add(new TableMigrationOperation("drop", notFoundMapping.Name));
    //     }
    //     
    //     foreach (var notFoundType in notFoundTypes)
    //     {
    //         var newTableProps = DbSchemaExtractor.ExtractTableProperties(notFoundType);
    //         operations.Add(new TableMigrationOperation("add", newTableProps.Name, newTableMapping: newTableProps));
    //     }
    //
    //     return operations;
    // }
}
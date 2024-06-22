using System.Reflection;
using OrmLibrary.Attributes;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations;
using OrmLibrary.Migrations.MigrationOperations.Tables;

namespace OrmLibrary.Migrations;

public static class MigrationManager
{
    private static readonly TableComparer TableComparer = new();
    
    public static List<TableMigrationOperation> CheckForChanges1(CurrentEntityModels currentEntityModels)
    {
        var operations = new List<TableMigrationOperation>();
        var notFoundTypes = OrmContext.MappedTypes.ToHashSet();

        foreach (var lastEntityMapping in currentEntityModels.EntitiesMappings.Values)
        {
            if (lastEntityMapping.AssociatedType is null)
            {
                // TODO: maybe different type, same table name?
                operations.Add(new TableMigrationOperation("drop", lastEntityMapping.Name));
            }
            else
            {
                // TODO: here decide if it should be compared (based on last write date of the file)
                notFoundTypes.Remove(lastEntityMapping.AssociatedType);
                
                var currentEntityMapping = DbSchemaExtractor.ExtractTableProperties(lastEntityMapping.AssociatedType);
                
                operations.AddRange(TableComparer.CompareTables(lastEntityMapping, currentEntityMapping));
            }
        }

        operations.AddRange(notFoundTypes
            .Select(DbSchemaExtractor.ExtractTableProperties)
            .Select(newTableProps => new TableMigrationOperation("add", newTableProps.Name, newTableMapping: newTableProps)));

        return operations;
    }
    
    // TODO: delete this
    public static List<TableMigrationOperation> CheckForChanges(CurrentEntityModels currentEntityModels)
    {
        // count mai mic la curent => s-a sters o tabela => gaseste care s-a sters dupa TableName && AssociatedType
        // count mai mare la curent => s-a adaugat o tabela => gaseste care s-a adaugat dupa TableName && AssociatedType
        
        // itereaza current mapped entities
            // daca e in last state mapped entities dupa TableName
                // check la last write date
                    // daca e <= decat last update date
                        // continue
                    // daca e >= decat last update date
                        // fa props check
            // daca nu e
                // e rename la table?
                // e table nou?
        // daca nu e in mapped entities dupa

        var operations = new List<TableMigrationOperation>();
        
        var notFoundTypes = OrmContext.MappedTypes.ToHashSet();
        var notFoundMappings = new HashSet<TableProperties>();
        
        foreach (var lastEntityMapping in currentEntityModels.EntitiesMappings.Values)
        {
            if (OrmContext.MappedTypes.Contains(lastEntityMapping.AssociatedType))
            {
                notFoundTypes.Remove(lastEntityMapping.AssociatedType);
                
                var currentEntityMapping = DbSchemaExtractor.ExtractTableProperties(lastEntityMapping.AssociatedType);
                
                TableComparer.CompareTables(lastEntityMapping, currentEntityMapping);
            }
            else
            {
                notFoundMappings.Add(lastEntityMapping);
            }
        }
        
        var entityNames = notFoundTypes.ToDictionary(type => type.GetCustomAttribute<TableAttribute>()!.Name);
        
        foreach (var lastEntityMapping in notFoundMappings)
        {
            if (entityNames.TryGetValue(lastEntityMapping.Name, out var value))
            {
                notFoundMappings.Remove(lastEntityMapping);
                notFoundTypes.Remove(value);
                
                var currentEntityMapping = DbSchemaExtractor.ExtractTableProperties(lastEntityMapping.AssociatedType);
                operations.AddRange(TableComparer.CompareTables(lastEntityMapping, currentEntityMapping));
            }
        }

        foreach (var notFoundMapping in notFoundMappings)
        {
            operations.Add(new TableMigrationOperation("drop", notFoundMapping.Name));
        }
        
        foreach (var notFoundType in notFoundTypes)
        {
            var newTableProps = DbSchemaExtractor.ExtractTableProperties(notFoundType);
            operations.Add(new TableMigrationOperation("add", newTableProps.Name, newTableMapping: newTableProps));
        }

        return operations;
    }
}
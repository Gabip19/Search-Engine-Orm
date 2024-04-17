using System.Reflection;
using OrmLibrary.Attributes;
using OrmLibrary.MigrationOperations;

namespace OrmLibrary;

public static class MigrationManager
{
    public static void CheckForChanges(CurrentEntityModels currentEntityModels)
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

        var notFoundTypes = OrmContext.MappedTypes.ToHashSet();
        var notFoundMappings = new HashSet<TableProperties>();
        
        foreach (var lastEntityMapping in currentEntityModels.EntitiesMappings)
        {
            if (OrmContext.MappedTypes.Contains(lastEntityMapping.AssociatedType))
            {
                notFoundTypes.Remove(lastEntityMapping.AssociatedType);
                
                // if file has changes
                var currentEntityMapping = DbSchemaExtractor.ExtractTableProperties(lastEntityMapping.AssociatedType);
                
                // compare mappings
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
                
                // compare mappings
            }
        }

        foreach (var notFoundMapping in notFoundMappings)
        {
            // drop table
        }
        
        foreach (var notFoundType in notFoundTypes)
        {
            // add new type
        }
    }

    private static MigrationOperation DiffCheck(TableProperties lastState, TableProperties currentState)
    {
        var lastStateColumns = lastState.Columns.ToDictionary(properties => properties.Name);
        
        foreach (var currentStateColumn in currentState.Columns)
        {
            if (lastStateColumns.TryGetValue(currentStateColumn.Name, out var lastStateColumn))
            {
                // compare mappings
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
                
                // compare mappings
            }
        }

        foreach (var notFoundMapping in notFoundMappings)
        {
            // drop table
        }
        
        foreach (var notFoundType in notFoundTypes)
        {
            // add new type
        }
    }
}
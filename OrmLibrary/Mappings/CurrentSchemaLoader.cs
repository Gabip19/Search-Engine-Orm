using System.Text.Json;
using OrmLibrary.Attributes;
using OrmLibrary.Extensions;
using OrmLibrary.Serialization;

namespace OrmLibrary.Mappings;

public static class CurrentSchemaLoader
{
    private const string CurrentSchemaFileName = "current_db_schema.json";
    private static readonly SchemaSerializer SchemaSerializer = new();
    
    public static CurrentEntityModels LoadCurrentSchema(string schemasDirPath)
    {
        var fileSchemaPath = $"{schemasDirPath}{Path.DirectorySeparatorChar}{CurrentSchemaFileName}";

        if (File.Exists(fileSchemaPath))
        {
            var json = File.ReadAllText(fileSchemaPath);
            return SchemaSerializer.DeserializeCurrentEntityModels(json)!;
        }

        var mappingEntities = OrmContext.DomainAssemblies.Select(x => x.GetDecoratedTypes(typeof(TableAttribute)))
            .Aggregate((types, enumerable) => enumerable.Concat(types));

        var currentModel = new CurrentEntityModels
        {
            EntitiesMappings = new MappedEntitiesCollection(DbSchemaExtractor.ExtractTablesProperties(mappingEntities)),
            CurrentDbVersion = 1,
            LastDbUpdate = DateTime.UtcNow,
            HasChanged = true
        };

        return currentModel;
    }
}
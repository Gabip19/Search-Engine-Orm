using System.Text.Json;
using OrmLibrary.Attributes;
using OrmLibrary.Extensions;

namespace OrmLibrary;

public static class CurrentSchemaLoader
{
    private const string CurrentSchemaFileName = "current_db_schema.json";
    
    public static CurrentEntityModels LoadCurrentSchema(string schemasDirPath)
    {
        var fileSchemaPath = $"{schemasDirPath}{Path.DirectorySeparatorChar}{CurrentSchemaFileName}";

        if (File.Exists(fileSchemaPath))
        {
            var json = File.ReadAllText(fileSchemaPath);
            return JsonSerializer.Deserialize<CurrentEntityModels>(json)!;
        }

        var mappingEntities = OrmContext.DomainAssemblies.Select(x => x.GetDecoratedTypes(typeof(TableAttribute)))
            .Aggregate((types, enumerable) => enumerable.Concat(types));

        var currentModel = new CurrentEntityModels
        {
            EntitiesMappings = DbSchemaExtractor.ExtractTablesProperties(mappingEntities),
            CurrentDbVersion = 1,
            LastDbUpdate = DateTime.UtcNow
        };

        return currentModel;
    }
}
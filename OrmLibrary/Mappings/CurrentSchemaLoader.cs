using OrmLibrary.Serialization;

namespace OrmLibrary.Mappings;

public static class CurrentSchemaLoader
{
    private const string CurrentSchemaFileName = "current_db_schema.json";
    private static readonly SchemaSerializer SchemaSerializer = new();
    
    public static CurrentEntityModels? LoadCurrentSchema(string schemasDirPath)
    {
        var fileSchemaPath = $"{schemasDirPath}{Path.DirectorySeparatorChar}{CurrentSchemaFileName}";

        if (!File.Exists(fileSchemaPath)) return null;
        
        var json = File.ReadAllText(fileSchemaPath);
        return SchemaSerializer.DeserializeCurrentEntityModels(json)!;
    }
}
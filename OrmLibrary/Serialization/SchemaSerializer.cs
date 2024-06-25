using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations;
using OrmLibrary.Serialization.Converters;

namespace OrmLibrary.Serialization;

public class SchemaSerializer
{
    private readonly JsonSerializerSettings _jsonSerializerSettings;
    
    public SchemaSerializer()
    {
        _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        _jsonSerializerSettings.Converters.Add(new CustomColumnPropertiesConverter());
        _jsonSerializerSettings.Converters.Add(new CustomTablePropertiesConverter());
        _jsonSerializerSettings.Converters.Add(new CustomTablePropertiesDtoConverter());
        _jsonSerializerSettings.Converters.Add(new CustomCurrentEntityModelsConverter());
        _jsonSerializerSettings.Converters.Add(new MigrationOperationsCollectionConverter());
        _jsonSerializerSettings.Converters.Add(new ColumnsOperationsCollectionConverter());
        _jsonSerializerSettings.Converters.Add(new ConstraintMigrationOperationConverter());
    }
    
    public string SerializeCurrentEntityModels(CurrentEntityModels currentEntityModels)
    {
        return JsonConvert.SerializeObject(currentEntityModels, _jsonSerializerSettings);
    }

    public string SerializeCollection(MigrationOperationsCollection operations)
    {
        return JsonConvert.SerializeObject(operations, _jsonSerializerSettings);
    }
    
    public CurrentEntityModels? DeserializeCurrentEntityModels(string json)
    {
        return JsonConvert.DeserializeObject<CurrentEntityModels>(json, _jsonSerializerSettings);
    }
}
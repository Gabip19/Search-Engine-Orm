using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrmLibrary.Mappings;
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
    }
    
    public string SerializeCurrentEntityModels(CurrentEntityModels currentEntityModels)
    {
        return JsonConvert.SerializeObject(currentEntityModels, _jsonSerializerSettings);
    }
    
    public CurrentEntityModels? DeserializeCurrentEntityModels(string json)
    {
        return JsonConvert.DeserializeObject<CurrentEntityModels>(json, _jsonSerializerSettings);
    }
}
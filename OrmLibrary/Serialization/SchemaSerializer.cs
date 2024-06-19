using Newtonsoft.Json;
using OrmLibrary.Mappings;
using OrmLibrary.Serialization.Converters;

namespace OrmLibrary.Serialization;

public class SchemaSerializer
{
    public string SerializeTable(TableProperties table)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        settings.Converters.Add(new CustomTablePropertiesConverter());
        settings.Converters.Add(new CustomColumnPropertiesConverter());

        return JsonConvert.SerializeObject(table, settings);
    }
}
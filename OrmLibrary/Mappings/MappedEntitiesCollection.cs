namespace OrmLibrary.Mappings;

public class MappedEntitiesCollection
{
    private readonly IDictionary<string, TableProperties> _entitiesMappings = new Dictionary<string, TableProperties>();

    public MappedEntitiesCollection(IEnumerable<TableProperties> mappingList)
    {
        foreach (var tableProperties in mappingList)
        {
            _entitiesMappings.Add(tableProperties.AssociatedType!.Name, tableProperties);
        }
    }

    public MappedEntitiesCollection(IDictionary<string, TableProperties> tablePropertiesMap)
    {
        _entitiesMappings = tablePropertiesMap;
    }

    public TableProperties this[string tablePropsKey] => _entitiesMappings[tablePropsKey];

    public TableProperties this[Type tablePropsType] => _entitiesMappings[tablePropsType.Name];

    public ICollection<TableProperties> Values => _entitiesMappings.Values;
}
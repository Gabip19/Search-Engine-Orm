namespace OrmLibrary.Mappings;

public class CurrentEntityModels
{
    public DateTime LastDbUpdate { get; set; }
    public int CurrentDbVersion { get; set; }
    public IDictionary<Type, TableProperties> EntitiesMappings { get; internal init; } = new Dictionary<Type, TableProperties>();
    public bool HasChanged { get; set; }
}
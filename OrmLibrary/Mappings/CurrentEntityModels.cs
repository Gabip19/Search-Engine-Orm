namespace OrmLibrary.Mappings;

public class CurrentEntityModels
{
    public DateTime LastDbUpdate { get; set; }
    public int CurrentDbVersion { get; set; }
    public MappedEntitiesCollection EntitiesMappings { get; internal set; } = null!;
    public bool HasChanged { get; set; }
}
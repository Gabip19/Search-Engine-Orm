namespace OrmLibrary;

public class CurrentEntityModels
{
    public DateTime LastDbUpdate { get; set; }
    public int CurrentDbVersion { get; set; }
    public ICollection<TableProperties> EntitiesMappings { get; set; }
    public bool HasChanged { get; set; }
}
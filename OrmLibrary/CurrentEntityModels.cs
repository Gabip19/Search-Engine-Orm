namespace OrmLibrary;

public class CurrentEntityModels
{
    public DateTime LastDbUpdate { get; set; }
    public int CurrentDbVersion { get; set; }
    public IEnumerable<TableProperties> EntitiesMappings { get; set; }
}
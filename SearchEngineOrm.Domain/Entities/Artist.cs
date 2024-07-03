using OrmLibrary.Attributes;

namespace SearchEngineOrm.Domain.Entities;

[Table("Artists")]
public class Artist
{
    [PrimaryKey]
    public string LastName { get; set; }

    [PrimaryKey]
    public string FirstName { get; set; }
    
    [Unique]
    public string StageName { get; set; }
    
    public int SongsNum { get; set; }
}
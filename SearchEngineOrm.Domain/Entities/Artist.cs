using OrmLibrary.Attributes;

namespace SearchEngineOrm.Domain.Entities;

[Table("Artists")]
public class Artist
{
    [PrimaryKey]
    public string ArtistName { get; set; }
    [PrimaryKey]
    public string Description { get; set; }
    public int Songs { get; set; }
}
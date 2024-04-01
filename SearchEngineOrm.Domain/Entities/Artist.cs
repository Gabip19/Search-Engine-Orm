using OrmLibrary.Attributes;

namespace SearchEngineOrm.Domain.Entities;

[Table("Artists")]
public class Artist
{
    public string ArtistName { get; set; }
    public string Description { get; set; }
    public int Songs { get; set; }
}
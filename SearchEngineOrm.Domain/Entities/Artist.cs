using OrmLibrary.Attributes;
using OrmLibrary.Attributes.Relational;

namespace SearchEngineOrm.Domain.Entities;

[Table("Artists")]
public class Artist
{
    [PrimaryKey]
    public string Name { get; set; }
    
    [PrimaryKey]
    public string Description { get; set; }
    
    [OneToMany(reversedPropertyName: nameof(Song.ArtistReversed))]
    public IList<Song> Songs { get; set; }
    
    public int SongsNum { get; set; }
}
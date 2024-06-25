using OrmLibrary.Attributes;
using OrmLibrary.Attributes.Relational;

namespace SearchEngineOrm.Domain.Entities;

[Table("Artists")]
public class Artist
{
    [PrimaryKey]
    // [Column(name: "NameNew")]
    public string Name { get; set; }
    
    // [PrimaryKey]
    // public string Description { get; set; }
    
    [OneToMany(reversedPropertyName: nameof(Song.MainArtist))]
    public IList<Song> MainSongs { get; set; }
    
    // [ManyToMany(reversedPropertyName: nameof(Song.Artists))]
    // public IList<Song> Songs { get; set; }
    
    [PrimaryKey]
    public int SongsNum { get; set; }
}
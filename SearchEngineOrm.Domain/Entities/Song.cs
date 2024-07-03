using OrmLibrary.Attributes;
using OrmLibrary.Attributes.Relational;

namespace SearchEngineOrm.Domain.Entities;

[Table(name: "Songs")]
public class Song
{
    [PrimaryKey]
    public int TrackId { get; set; }
    
    [Column]
    [PrimaryKey]
    public string SongTitle { get; set; } = null!;

    [ManyToOne(columnsNamesPrefix: "Main")]
    public Artist MainArtist { get; set; }
    
    [Column(name: "ProducerNameFK")]
    [ForeignKey(typeof(Artist), nameof(Artist.StageName))]
    public string ProducerName { get; set; }

    // [ForeignKey(typeof(Genre), nameof(Genre.GenreId))]
    // public Guid GenreId { get; set; }
    
    public char Md5Char { get; set; }

    public int? RadioAppearances { get; set; }
    
    public bool IsExplicit { get; set; }
    
    public float PopularityScore { get; set; }
}
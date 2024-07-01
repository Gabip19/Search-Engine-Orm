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
    
    // [MaxLength(length: 15)]
    // public string? LengthTest { get; set; } = null!;
    
    // public int TrackNo { get; set; }
    
    // [Column(name: "Price")]
    // [Unique]
    // public double SellPrice { get; set; }

    [ManyToOne(columnsNamesPrefix: "Main")]
    public Artist MainArtist { get; set; }

    // [ManyToMany(reversedPropertyName: nameof(Artist.Songs))]
    // public IList<Artist> Artists { get; set; }
    
    [Column(name: "ArtistNameFK2")]
    [ForeignKey(typeof(Artist), nameof(Artist.Name))]
    public string ArtistName { get; set; }

    // [ForeignKey(typeof(Genre), nameof(Genre.GenreId))]
    // public Guid GenreId { get; set; }
    
    // [Unique]
    public string Md5Char { get; set; }

    // public char Test { get; set; }
    
    public bool IsExplicit { get; set; }
    
    // public float? PopularityScore { get; set; }

    // [OneToOne]
    // public SongInfo SongInfo { get; set; }
    
    // [Unmapped]
    // public string RandomStuff { get; set; } = null!;
}
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
    
    [MaxLength(length: 15)]
    public string? LengthTest { get; set; } = null!;
    
    public int TrackNo { get; set; }
    
    [Column(name: "Price")]
    [Unique]
    public double SellPrice { get; set; }

    [ManyToOne]
    public Artist Artist { get; set; }
    
    [ForeignKey]
    public string ArtistName { get; set; }
    
    public char Md5Char { get; set; }
    
    public bool IsExplicit { get; set; }
    
    public float? PopularityScore { get; set; }
    
    [Unmapped]
    public string RandomStuff { get; set; } = null!;
}
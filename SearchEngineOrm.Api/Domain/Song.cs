using OrmLibrary.Attributes;

namespace SearchEngineOrm.Api.Domain;

[Table(name: "Songs")]
public class Song
{
    [PrimaryKey]
    public int TrackId { get; set; }
    
    [Column]
    public string SongTitle { get; set; } = null!;
    
    public int TrackNo { get; set; }
    
    [Column(name: "Price")]
    public double SellPrice { get; set; }
    
    public char Md5Char { get; set; }
    
    public bool IsExplicit { get; set; }
    
    public float? PopularityScore { get; set; }

    [Unmapped]
    public string RandomStuff { get; set; } = null!;
}
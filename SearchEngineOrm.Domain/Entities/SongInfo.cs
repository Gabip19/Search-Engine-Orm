using OrmLibrary.Attributes;

namespace SearchEngineOrm.Domain.Entities;

[Table("SongInfo")]
public class SongInfo
{
    public Guid SongId { get; set; }
    public string Info { get; set; }
}
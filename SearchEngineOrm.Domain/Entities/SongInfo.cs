using OrmLibrary.Attributes;
using OrmLibrary.Attributes.Relational;

namespace SearchEngineOrm.Domain.Entities;

// [Table("SongInfo")]
public class SongInfo
{
    public Guid SongId { get; set; }
    public string Info { get; set; }

    [OneToOne]
    public Song Song { get; set; }
}
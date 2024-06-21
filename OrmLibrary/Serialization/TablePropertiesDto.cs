using OrmLibrary.Mappings;

namespace OrmLibrary.Serialization;

public class TablePropertiesDto
{
    public TableProperties UnlinkedTableProperties { get; set; }
    public IList<ForeignKeyGroupDto> KeyGroupsDtos { get; set; }
}
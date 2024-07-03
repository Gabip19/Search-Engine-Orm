using OrmLibrary.Mappings;

namespace OrmLibrary.Abstractions;

public interface IDbSchemaExtractor
{
    public ICollection<TableProperties> ExtractTablesProperties(IEnumerable<Type> entitiesTypes);
    
    public TableProperties ExtractTableProperties(Type entityType);
}
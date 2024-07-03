using OrmLibrary.Mappings;

namespace OrmLibrary.Abstractions;

public interface IMigrationManager
{
    public void CheckForSchemaUpdates(CurrentEntityModels? currentEntityModels);
    public void UpdateDatabase();
}
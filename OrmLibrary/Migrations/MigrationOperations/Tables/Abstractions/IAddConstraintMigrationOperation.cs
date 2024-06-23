using OrmLibrary.Constraints;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public interface IAddConstraintMigrationOperation : IConstraintMigrationOperation
{
    public ITableConstraint NewConstraint { get; set; }
}
using OrmLibrary.Enums;

namespace OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;

public interface IAddConstraintMigrationOperation : IConstraintMigrationOperation
{
    public TableConstraintType ConstraintType { get; set; }
}
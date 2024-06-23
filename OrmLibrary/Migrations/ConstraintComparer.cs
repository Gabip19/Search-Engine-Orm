using OrmLibrary.Constraints;
using OrmLibrary.Extensions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;
using TableOperationsFactory = OrmLibrary.Migrations.MigrationOperations.Tables.TableMigrationOperationsFactory;

namespace OrmLibrary.Migrations;

public class ConstraintComparer
{
    public IConstraintMigrationOperation? CompareConstraints(ITableConstraint oldConstraint, ITableConstraint newConstraint)
    {
        if (oldConstraint is ForeignKeyConstraint oldFkConstraint && newConstraint is ForeignKeyConstraint newFkConstraint)
        {
            return CompareForeignKeyConstraints(oldFkConstraint, newFkConstraint);
        }

        return null;
    }

    private AlterForeignKeyConstraintOperation? CompareForeignKeyConstraints(ForeignKeyConstraint oldConstraint, ForeignKeyConstraint newConstraint)
    {
        var oldConstraintKeyPairs = oldConstraint.ForeignKeyGroup.KeyPairs;
        var newConstraintKeyPairs = newConstraint.ForeignKeyGroup.KeyPairs;

        var shouldChange = false;
        
        if (oldConstraintKeyPairs.Count == newConstraintKeyPairs.Count)
        {
            if (newConstraintKeyPairs.Where((t, i) => 
                    !oldConstraintKeyPairs[i].MainColumn.IsSameColumnAs(t.MainColumn) || 
                    !oldConstraintKeyPairs[i].ReferencedColumn.IsSameColumnAs(t.ReferencedColumn))
                .Any())
            {
                shouldChange = true;
            }
        }
        else
        {
            shouldChange = true;
        }

        return shouldChange
            ? TableOperationsFactory.NewAlterForeignKeyOperation(newConstraint)
            : null;
    }
}
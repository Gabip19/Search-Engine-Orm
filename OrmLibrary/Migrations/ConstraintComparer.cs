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
            for (var i = 0; i < newConstraintKeyPairs.Count; i++)
            {
                var newKeyPair = newConstraintKeyPairs[i];
                var oldKeyPair = oldConstraintKeyPairs[i];

                if ((oldKeyPair.MainColumn.IsSameColumnAs(newKeyPair.MainColumn) ||
                    oldKeyPair.ReferencedColumn.IsSameColumnAs(newKeyPair.ReferencedColumn)) && 
                    oldKeyPair.MainColumn.SqlColumnType == newKeyPair.MainColumn.SqlColumnType) continue;
                
                shouldChange = true;
                break;
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
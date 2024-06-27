using System.Text;
using OrmLibrary.Converters;
using OrmLibrary.Enums;
using OrmLibrary.Mappings;
using OrmLibrary.Migrations.MigrationOperations.Columns.Concrete;
using OrmLibrary.Migrations.MigrationOperations.Tables;
using OrmLibrary.Migrations.MigrationOperations.Tables.Abstractions;
using OrmLibrary.Migrations.MigrationOperations.Tables.Concrete;

namespace OrmLibrary.SqlServer;

public class SqlServerDdlGenerator : ISqlDdlGenerator
{
    public string GenerateSql(IAddTableMigrationOperation operation)
    {
        var sqlBuilder = new StringBuilder();
        var primaryKeys = new List<string>();
        
        sqlBuilder.Append($"CREATE TABLE {operation.TableName} (\n");

        foreach (var column in operation.Columns)
        {
            sqlBuilder.Append(GetColumnString(column));
            sqlBuilder.Append(",\n");

            if (column.IsPrimaryKeyColumn)
            {
                primaryKeys.Add(column.Name);
            }
        }

        sqlBuilder.Append($"CONSTRAINT PK_{operation.TableName} PRIMARY KEY (");

        foreach (var primaryKey in primaryKeys)
        {
            sqlBuilder.Append($"{primaryKey}, ");
        }

        sqlBuilder.Length -= 2;
        sqlBuilder.Append(")\n);");
        
        return sqlBuilder.ToString();
    }

    public string GenerateSql(IDropTableMigrationOperation operation)
    {
        return $"DROP TABLE {operation.TableName};";
    }

    public string GenerateSql(IAlterTableStructureMigrationOperation operation)
    {
        var sqlBuilder = new StringBuilder();

        foreach (var subOperation in operation.ColumnOperations.AddColumnOperations)
        {
            sqlBuilder.Append(GenerateSql((dynamic)subOperation));
            sqlBuilder.Append("\n\n");
        }
        
        foreach (var subOperation in operation.ColumnOperations.DropColumnOperations)
        {
            sqlBuilder.Append(GenerateSql((dynamic)subOperation));
            sqlBuilder.Append("\n\n");
        }
        
        foreach (var subOperation in operation.ColumnOperations.AlterColumnOperations.OfType<RenameColumnOperation>())
        {
            sqlBuilder.Append(GenerateSql(subOperation));
            sqlBuilder.Append("\n\n");
        }
        
        foreach (var subOperation in operation.ColumnOperations.AlterColumnOperations
                     .Where(op => op is not RenameColumnOperation && op is not ChangeNullabilityColumnOperation))
        {
            sqlBuilder.Append(GenerateSql((dynamic)subOperation));
            sqlBuilder.Append("\n\n");
        }
        
        foreach (var subOperation in operation.ColumnOperations.AlterColumnOperations.OfType<ChangeNullabilityColumnOperation>())
        {
            sqlBuilder.Append(GenerateSql(subOperation));
            sqlBuilder.Append("\n\n");
        }

        sqlBuilder.Length -= 2;

        return sqlBuilder.ToString();
    }

    public string GenerateSql(AddColumnOperation operation)
    {
        return $"ALTER TABLE {operation.TableName}\nADD {GetColumnString(operation.NewColumnProps)};";
    }

    private static string GetColumnString(ColumnProperties columnProps)
    {
        return $"{columnProps.Name} {GetColumnTypeString(columnProps.SqlColumnType, columnProps.MaxLength)} {(columnProps.IsNullable ? "" : "NOT NULL")}";
    }

    public string GenerateSql(DropColumnOperation operation)
    {
        return $"ALTER TABLE {operation.TableName}\nDROP COLUMN {operation.ColumnName};";
    }

    public string GenerateSql(ChangeNullabilityColumnOperation operation)
    {
        return $"ALTER TABLE {operation.TableName}\nALTER COLUMN {operation.ColumnName} {GetColumnTypeString(operation.ColumnType)} {(operation.IsNullable ? "NULL" : "NOT NULL")};";
    }

    public string GenerateSql(RenameColumnOperation operation)
    {
        return $"EXEC sp_rename '{operation.TableName}.{operation.ColumnName}', '{operation.NewColumnName}', 'COLUMN';";
    }

    public string GenerateSql(ChangeTypeColumnOperation operation)
    {
        return $"ALTER TABLE {operation.TableName}\nALTER COLUMN {operation.ColumnName} {GetColumnTypeString(operation.NewType)};";
    }

    public string GenerateSql(ChangeLengthColumnOperation operation)
    {
        return $"ALTER TABLE {operation.TableName}\nALTER COLUMN {operation.ColumnName} {GetColumnTypeString(operation.ColumnType, operation.Length)};";
    }

    public string GenerateSql(ChangePrimaryKeyColumnOperation operation)
    {
        return string.Empty;
    }

    public string GenerateSql(AddForeignKeyConstraintOperation operation)
    {
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append($"ALTER TABLE {operation.TableName}\nADD CONSTRAINT {operation.ConstraintName}\nFOREIGN KEY (");

        foreach (var column in operation.ForeignKeyGroupDto.Columns)
        {
            sqlBuilder.Append($"{column}, ");
        }

        sqlBuilder.Length -= 2;
        sqlBuilder.Append($") REFERENCES {operation.ForeignKeyGroupDto.ReferencedTableName}(");
        
        foreach (var column in operation.ForeignKeyGroupDto.ReferencedColumns)
        {
            sqlBuilder.Append($"{column}, ");
        }
        
        sqlBuilder.Length -= 2;
        sqlBuilder.Append(");");

        return sqlBuilder.ToString();
    }

    public string GenerateSql(AddUniqueConstraintOperation operation)
    {
        return $"ALTER TABLE {operation.TableName}\nADD CONSTRAINT {operation.ConstraintName} UNIQUE ({operation.ColumnName});";
    }

    public string GenerateSql(AlterForeignKeyConstraintOperation operation)
    {
        var dropConstraintSql = GenerateSql(new DropConstraintOperation
        {
            TableName = operation.TableName,
            ConstraintName = operation.ConstraintName,
            OperationType = TableOperationType.DropConstraint
        });

        var addFkConstraintSql = GenerateSql(new AddForeignKeyConstraintOperation(
            operation.TableName,
            TableOperationType.AddConstraint,
            operation.ConstraintName,
            TableConstraintType.ForeignKeyConstraint
        )
        {
            ForeignKeyGroupDto = operation.KeyGroupDto
        });

        return $"{dropConstraintSql}\n{addFkConstraintSql}";
    }

    public string GenerateSql(AlterPrimaryKeyConstraintOperation operation)
    {
        var sqlBuilder = new StringBuilder(GenerateSql(new DropConstraintOperation
        {
            TableName = operation.TableName,
            OperationType = TableOperationType.DropConstraint,
            ConstraintName = $"PK_{operation.TableName}"
        }));

        sqlBuilder.Append('\n');
        sqlBuilder.Append($"ALTER TABLE {operation.TableName}\nADD CONSTRAINT PK_{operation.TableName} PRIMARY KEY (");

        foreach (var column in operation.PrimaryKeyColumns)
        {
            sqlBuilder.Append($"{column}, ");
        }
        
        sqlBuilder.Length -= 2;
        sqlBuilder.Append(");");

        return sqlBuilder.ToString();
    }

    public string GenerateSql(IDropConstraintMigrationOperation operation)
    {
        return $"ALTER TABLE {operation.TableName}\nDROP CONSTRAINT {operation.ConstraintName};";
    }

    private static string GetColumnTypeString(SqlType type, int? length = null)
    {
        return type switch
        {
            SqlType.BigInt => "BIGINT",
            SqlType.Binary => length.HasValue ? $"BINARY({length.Value})" : "BINARY",
            SqlType.Bit => "BIT",
            SqlType.Char => length.HasValue ? $"CHAR({length.Value})" : "CHAR",
            SqlType.DateTime => "DATETIME",
            SqlType.DateTime2 => length.HasValue ? $"DATETIME2({length.Value})" : "DATETIME2",
            SqlType.Decimal => length.HasValue ? $"DECIMAL({length.Value})" : "DECIMAL",
            SqlType.Float => "FLOAT",
            SqlType.Int => "INT",
            SqlType.NChar => length.HasValue ? $"NCHAR({length.Value})" : "NCHAR",
            SqlType.NVarChar => length.HasValue ? $"NVARCHAR({length.Value})" : "NVARCHAR(MAX)",
            SqlType.Real => "REAL",
            SqlType.UniqueIdentifier => "UNIQUEIDENTIFIER",
            SqlType.SmallInt => "SMALLINT",
            SqlType.Timestamp => "TIMESTAMP",
            SqlType.TinyInt => "TINYINT",
            SqlType.VarBinary => length.HasValue ? $"VARBINARY({length.Value})" : "VARBINARY(MAX)",
            SqlType.VarChar => length.HasValue ? $"VARCHAR({length.Value})" : "VARCHAR(MAX)",
            SqlType.Date => "DATE",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
using OrmLibrary.Constraints;
using OrmLibrary.Enums;

namespace OrmLibrary.Mappings;

public class ColumnProperties
{
    public string Name { get; set; }
    public string TableName { get; set; }
    public string? PropertyName { get; set; }
    public Type LanguageNativeType { get; set; }
    public SqlType SqlColumnType { get; set; }
    public bool IsNullable { get; set; }
    public bool IsPrimaryKeyColumn { get; set; }
    public bool IsForeignKeyColumn { get; set; }
    public ForeignKeyGroup? ForeignKeyGroup { get; set; }
    public bool IsUnique { get; set; }
    public bool? IsFixedLength { get; set; }
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
    public object? DefaultValue { get; set; }
    public string? ComputedColumnSql { get; set; }

    public ColumnProperties() { }
    
    public ColumnProperties(ColumnProperties properties)
    {
        Name = properties.Name;
        TableName = properties.TableName;
        PropertyName = properties.PropertyName;
        LanguageNativeType = properties.LanguageNativeType;
        SqlColumnType = properties.SqlColumnType;
        IsNullable = properties.IsNullable;
        IsPrimaryKeyColumn = properties.IsPrimaryKeyColumn;
        IsForeignKeyColumn = properties.IsForeignKeyColumn;
        ForeignKeyGroup = properties.ForeignKeyGroup;
        IsUnique = properties.IsUnique;
        IsFixedLength = properties.IsFixedLength;
        MaxLength = properties.MaxLength;
        Precision = properties.Precision;
        DefaultValue = properties.DefaultValue;
        ComputedColumnSql = properties.ComputedColumnSql;
    }
}
using OrmLibrary.Enums;

namespace OrmLibrary;

public class ColumnProperties
{
    public string Name { get; set; }
    public string PropertyName { get; set; }
    public Type LanguageNativeType { get; set; }
    public SqlType SqlColumnType { get; set; }
    public bool IsNullable { get; set; }
    public bool IsPrimaryKeyColumn { get; set; }
    public bool IsUnique { get; set; }
    public bool? IsFixedLength { get; set; }
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
    public object? DefaultValue { get; set; }
    public string? DefaultValueSql { get; set; }
    public string? ComputedColumnSql { get; set; }
}
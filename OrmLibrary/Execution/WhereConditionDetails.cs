namespace OrmLibrary.Execution;

public class WhereConditionDetails
{
    public string PropertyName { get; set; }
    public string Operation { get; set; }
    public object Value { get; set; }
    public bool IsConstant { get; set; }
    public Type ValueType { get; set; }
    public string LogicalOperator { get; set; }
    public int GroupLevel { get; set; }
}
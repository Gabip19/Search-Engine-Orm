using System.Linq.Expressions;

namespace OrmLibrary.Execution.Parsers;

public class SelectExpressionVisitor : ExpressionVisitor
{
    public List<string> SelectedColumns { get; } = new();

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member.MemberType == System.Reflection.MemberTypes.Property)
        {
            SelectedColumns.Add(node.Member.Name);
        }
        return base.VisitMember(node);
    }

    protected override Expression VisitNew(NewExpression node)
    {
        foreach (var arg in node.Arguments)
        {
            Visit(arg);
        }
        return node;
    }
}
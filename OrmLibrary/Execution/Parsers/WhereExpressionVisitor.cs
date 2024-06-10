using System.Linq.Expressions;
using OrmLibrary.Execution;
using OrmLibrary.Execution.Query;

public class WhereExpressionVisitor : ExpressionVisitor
{
    private readonly List<WhereConditionDetails> _comparisons = new();
    private WhereConditionDetails _currentComparison;
    private int _currentGroupLevel;

    public List<WhereConditionDetails> Comparisons => _comparisons;

    protected override Expression VisitBinary(BinaryExpression node)
    {
        bool isLogicalOperator = node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse;

        if (isLogicalOperator)
        {
            _currentGroupLevel++;
        }

        Visit(node.Left);

        switch (node.NodeType)
        {
            case ExpressionType.AndAlso:
                _currentComparison = new WhereConditionDetails { LogicalOperator = "AND", GroupLevel = _currentGroupLevel };
                _comparisons.Add(_currentComparison);
                break;
            case ExpressionType.OrElse:
                _currentComparison = new WhereConditionDetails { LogicalOperator = "OR", GroupLevel = _currentGroupLevel };
                _comparisons.Add(_currentComparison);
                break;
            case ExpressionType.Equal:
                _currentComparison.Operation = "=";
                break;
            case ExpressionType.NotEqual:
                _currentComparison.Operation = "<>";
                break;
            case ExpressionType.GreaterThan:
                _currentComparison.Operation = ">";
                break;
            case ExpressionType.GreaterThanOrEqual:
                _currentComparison.Operation = ">=";
                break;
            case ExpressionType.LessThan:
                _currentComparison.Operation = "<";
                break;
            case ExpressionType.LessThanOrEqual:
                _currentComparison.Operation = "<=";
                break;
            default:
                throw new NotSupportedException($"The binary operator '{node.NodeType}' is not supported");
        }

        Visit(node.Right);

        if (isLogicalOperator)
        {
            _currentGroupLevel--;
        }

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member.MemberType == System.Reflection.MemberTypes.Property)
        {
            if (_currentComparison == null || _currentComparison.Operation == null)
            {
                _currentComparison = new WhereConditionDetails { GroupLevel = _currentGroupLevel };
                _comparisons.Add(_currentComparison);
            }

            if (node.Member.Name == "Length" && node.Expression is MemberExpression memberExpression)
            {
                // Handle Length property of strings
                _currentComparison.PropertyName = $"{memberExpression.Member.Name}.Length";
            }
            else
            {
                _currentComparison.PropertyName = node.Member.Name;
            }
        }

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (_currentComparison != null)
        {
            _currentComparison.Value = node.Value;
            _currentComparison.IsConstant = true;
            _currentComparison.ValueType = node.Type;
        }

        return node;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        switch (node.Method.Name)
        {
            case "StartsWith":
                HandleStringMethod(node, "LIKE", " + '%'");
                break;
            case "EndsWith":
                HandleStringMethod(node, "LIKE", "'%' + ");
                break;
            case "Contains":
                HandleStringMethod(node, "LIKE", "'%' + ", " + '%'");
                break;
            case "Equals":
                HandleEqualsMethod(node);
                break;
            case "Substring":
                HandleSubstringMethod(node);
                break;
            case "ToUpper":
                HandleToUpperMethod(node);
                break;
            case "ToLower":
                HandleToLowerMethod(node);
                break;
            case "IsNullOrEmpty":
                HandleIsNullOrEmptyMethod(node);
                break;
            case "IsNullOrWhiteSpace":
                HandleIsNullOrWhiteSpaceMethod(node);
                break;
            default:
                throw new NotSupportedException($"The method '{node.Method.Name}' is not supported");
        }

        return node;
    }

    private void HandleStringMethod(MethodCallExpression node, string sqlOperator, string suffix, string prefix = "")
    {
        Visit(node.Object);
        _currentComparison.Operation = sqlOperator;
        Visit(node.Arguments[0]);
        _currentComparison.Value = $"{prefix}{_currentComparison.Value}{suffix}";
    }

    private void HandleEqualsMethod(MethodCallExpression node)
    {
        Visit(node.Object);
        _currentComparison.Operation = "=";
        Visit(node.Arguments[0]);
    }

    private void HandleSubstringMethod(MethodCallExpression node)
    {
        Visit(node.Object);
        _currentComparison.Operation = "SUBSTRING";
        Visit(node.Arguments[0]);
    }

    private void HandleToUpperMethod(MethodCallExpression node)
    {
        Visit(node.Object);
        _currentComparison.Operation = "UPPER";
    }

    private void HandleToLowerMethod(MethodCallExpression node)
    {
        Visit(node.Object);
        _currentComparison.Operation = "LOWER";
    }

    private void HandleIsNullOrEmptyMethod(MethodCallExpression node)
    {
        Visit(node.Arguments[0]);
        _currentComparison.Operation = "IS NULL OR EMPTY";
    }

    private void HandleIsNullOrWhiteSpaceMethod(MethodCallExpression node)
    {
        Visit(node.Arguments[0]);
        _currentComparison.Operation = "IS NULL OR WHITESPACE";
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (_currentComparison != null)
        {
            _currentComparison.IsConstant = false;
            _currentComparison.ValueType = node.Type;
        }
        return base.VisitParameter(node);
    }
}
